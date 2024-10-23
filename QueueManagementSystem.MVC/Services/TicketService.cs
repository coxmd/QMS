using QueueManagementSystem.MVC.Models;
using Microsoft.EntityFrameworkCore;
using QueueManagementSystem.MVC.Data;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace QueueManagementSystem.MVC.Services;

public static class TicketStatus
{
    public const string InQueue = "In-Queue";
    public const string Called = "Called";
    public const string InService = "In-Service";
    public const string NoShow = "No-Show";
}

public class TicketService : ITicketService
{   
    private readonly IDbContextFactory<QueueManagementSystemContext> _dbFactory;

    private static readonly List<Ticket> NoShowTickets = new();
    private readonly IConfiguration _configuration;
    private readonly ILogger<TicketService> _logger;

    public TicketService(IDbContextFactory<QueueManagementSystemContext> dbFactory, IConfiguration configuration, ILogger<TicketService> logger)
    {
        _dbFactory = dbFactory;
        _configuration = configuration;
        _logger = logger;
    }

    private async Task<string> GenerateTicketNumberAsync()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        var today = DateTime.Today;
        var latestTicket = await context.QueuedTickets
            .Where(t => t.PrintTime.Date == today)
            .OrderByDescending(t => t.PrintTime)
            .FirstOrDefaultAsync();

        int nextNumber = (latestTicket == null) ? 1 : int.Parse(latestTicket.TicketNumber) + 1;
        return nextNumber.ToString("D4");
    }

    public async Task<List<Ticket>> GetAllQueuedTicketsAsync()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        return await context.QueuedTickets
            .OrderBy(t => t.PrintTime)
            .ToListAsync();
    }

    // Modify the TransferTicket method to reset the ServicePointAssignmentTime:
    public async Task TransferTicket(int ticketId, string newServiceName)
    {
        using var context = _dbFactory.CreateDbContext();
        var ticket = await context.QueuedTickets
            .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null)
        {
            throw new ArgumentException("Ticket not found", nameof(ticketId));
        }

        var newServicePoint = await context.ServicePoints
            .Include(sp => sp.Service)
            .FirstOrDefaultAsync(sp => sp.Service.Name == newServiceName);

        if (newServicePoint == null)
        {
            throw new ArgumentException("Service not found", nameof(newServiceName));
        }

        // Update service point details and reset the assignment time
        ticket.ServiceName = newServiceName;
        ticket.ServicePointId = newServicePoint.Id;
        ticket.Status = TicketStatus.InQueue;
        ticket.ServicePointAssignmentTime = DateTime.Now;  // Reset the timer for the new service point

        await context.SaveChangesAsync();

        // Trigger the TicketQueueAlteredEvent
        TicketQueueAlteredEvent?.Invoke(this, EventArgs.Empty);
    }

    public async Task<Ticket> GetActiveTicketAsync(int servicePointId, string serviceProviderId)
    {
        using var context = _dbFactory.CreateDbContext();

        // Build the base query
        var query = context.QueuedTickets
            .AsNoTracking()
            .Where(t => t.ServicePointId == servicePointId);

        // Add conditions based on whether we're in pooling mode
        if (!string.IsNullOrEmpty(serviceProviderId))
        {
            // In pooling mode, look for tickets locked by this service provider
            query = query.Where(t => t.IsLocked && t.LockedByServiceProviderId == serviceProviderId);
        }

        // Look for tickets that are either Called or In-Service status
        query = query.Where(t => t.Status == TicketStatus.Called || t.Status == TicketStatus.InService);

        // Order by status (so In-Service takes precedence over Called) and then by call time
        var activeTicket = await query
            .OrderByDescending(t => t.Status == TicketStatus.InService)
            .ThenBy(t => t.PrintTime)
            .FirstOrDefaultAsync();

        return activeTicket;
    }

    public async Task<bool> HasActiveTicketAsync(int servicePointId, string? serviceProviderId = null, string? excludeTicketNumber = null)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        var query = context.QueuedTickets.AsQueryable();

        if (serviceProviderId != null)
        {
            // For pooled mode, check by service provider
            query = query.Where(t => t.LockedByServiceProviderId == serviceProviderId);
        }
        else
        {
            // For non-pooled mode, check by service point
            query = query.Where(t => t.ServicePointId == servicePointId);
        }

        // Exclude the current ticket if specified
        if (!string.IsNullOrEmpty(excludeTicketNumber))
        {
            query = query.Where(t => t.TicketNumber != excludeTicketNumber);
        }

        return await query.AnyAsync(t =>
            t.Status == TicketStatus.Called ||
            t.Status == "In-Service");
    }

    //public async Task CallTicketAsync(string ticketNumber, string calledServicePointName)
    //{
    //    await using var context = await _dbFactory.CreateDbContextAsync();
    //    var ticket = await context.QueuedTickets.FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber);

    //    if (ticket != null)
    //    {
    //        ticket.Status = TicketStatus.Called;
    //        await context.SaveChangesAsync();

    //        TicketCalledFromQueueEvent?.Invoke(this, (ticket.TicketNumber, calledServicePointName));
    //        TicketQueueAlteredEvent?.Invoke(this, EventArgs.Empty);
    //    }
    //    else
    //    {
    //        throw new ArgumentException($"Ticket with number {ticketNumber} not found.");
    //    }
    //}

    public async Task CallTicketAsync(string ticketNumber, string calledServicePointName, int servicePointId, string? serviceProviderId = null)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        // Check if there's already an active ticket
        bool hasActiveTicket = await HasActiveTicketAsync(servicePointId, serviceProviderId);
        if (hasActiveTicket)
        {
            throw new InvalidOperationException("Cannot call a new ticket while another ticket is active.");
        }

        var ticket = await context.QueuedTickets.FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber);
        if (ticket != null)
        {
            ticket.Status = TicketStatus.Called;
            await context.SaveChangesAsync();
            TicketCalledFromQueueEvent?.Invoke(this, (ticket.TicketNumber, calledServicePointName));
            TicketQueueAlteredEvent?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            throw new ArgumentException($"Ticket with number {ticketNumber} not found.");
        }
    }


    public async Task<Ticket?> GetTicketFromQueueAsync(string serviceName, string calledServicePointName)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        var ticket = await context.QueuedTickets
            .Where(t => t.ServiceName == serviceName && t.Status == TicketStatus.InQueue)
            .OrderBy(t => t.PrintTime)
            .FirstOrDefaultAsync();

        if (ticket != null)
        {
            ticket.Status = TicketStatus.Called;
            await context.SaveChangesAsync();
            TicketCalledFromQueueEvent?.Invoke(this, (ticket.TicketNumber, calledServicePointName));
            TicketQueueAlteredEvent?.Invoke(this, EventArgs.Empty);
        }

        return ticket;
    }

    public async Task MoveTicketToEndOfQueueAsync(int ticketId)
    {
        using var context = await _dbFactory.CreateDbContextAsync();

        // Get the ticket to move
        var ticket = await context.QueuedTickets
            .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null) return;

        // Get the maximum print time of all current tickets
        var maxPrintTime = await context.QueuedTickets
            .Where(t => t.Status == TicketStatus.InQueue)
            .MaxAsync(t => t.PrintTime);

        // Set the print time to slightly later than the latest ticket
        ticket.PrintTime = maxPrintTime.AddSeconds(1);

        await context.SaveChangesAsync();
        TicketQueueAlteredEvent?.Invoke(this, EventArgs.Empty);
    }

    public async Task UpdateTicketNoShowStatusAsync(int ticketId, bool wasNoShow, DateTime? noShowTime)
    {
        try
        {
            await using var context = await _dbFactory.CreateDbContextAsync();

            // Find the ticket
            var ticket = await context.QueuedTickets.FindAsync(ticketId);
            if (ticket == null)
            {
                throw new ArgumentException($"Ticket with ID {ticketId} not found.", nameof(ticketId));
            }

            // Update no-show status
            ticket.WasNoShow = wasNoShow;
            ticket.LastNoShowTime = noShowTime;

            // If this is a new no-show, log it
            if (wasNoShow && noShowTime.HasValue)
            {
                _logger.LogInformation(
                    "Ticket {TicketNumber} marked as no-show at {NoShowTime}",
                    ticket.TicketNumber,
                    noShowTime.Value
                );
            }

            // Save changes
            await context.SaveChangesAsync();

            // Trigger queue altered event since the ticket's status has changed
            TicketQueueAlteredEvent?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating no-show status for ticket {TicketId}", ticketId);
            throw;
        }
    }

    public async Task MoveTicketToEndOfServicePointQueueAsync(int ticketId, int servicePointId)
    {
        using var context = await _dbFactory.CreateDbContextAsync();

        // Get the ticket to move
        var ticket = await context.QueuedTickets
            .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null) return;

        // Get the maximum print time of tickets for this service point
        var maxPrintTime = await context.QueuedTickets
            .Where(t => t.ServicePointId == servicePointId && t.Status == TicketStatus.InQueue)
            .MaxAsync(t => t.PrintTime);

        // Set the print time to slightly later than the latest ticket
        ticket.PrintTime = maxPrintTime.AddSeconds(1);

        await context.SaveChangesAsync();
        TicketQueueAlteredEvent?.Invoke(this, EventArgs.Empty);
    }

    public Ticket? GetNoShowTicket(string serviceName, string calledServicePointName)
    {
        var ticket = NoShowTickets.Where(t => t.ServiceName == serviceName).FirstOrDefault();
        
        if (ticket!=null)
        {
            NoShowTickets.Remove(ticket);
            ticket.Status = TicketStatus.Called;
            using var context = _dbFactory.CreateDbContext();
            context.Update(ticket);
            context.SaveChanges();

            TicketCalledFromQueueEvent?.Invoke(this, (ticket.TicketNumber, calledServicePointName));

            TicketQueueAlteredEvent?.Invoke(this, EventArgs.Empty);
        }

        return ticket;
    }

    public async Task<List<Ticket>> GetTicketsByServiceNameAsync(string serviceName)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        return await context.QueuedTickets
            .Where(t => t.ServiceName == serviceName)
            .OrderBy(t => t.PrintTime)
            .ToListAsync();
    }

    public async Task<List<Ticket>> GetTicketsByServicePointIdAsync(int servicePointId)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();

        bool isPoolingEnabled = await IsPoolingEnabledAsync();

        if (isPoolingEnabled)
        {
            // If pooling is enabled, get the service name for this service point
            var servicePoint = await context.ServicePoints
                .Include(sp => sp.Service)
                .FirstOrDefaultAsync(sp => sp.Id == servicePointId);

            if (servicePoint != null)
            {
                // Return all tickets for this service
                return await context.QueuedTickets
                    .Where(t => t.ServiceName == servicePoint.Service.Name)
                    .OrderByDescending(t => t.IsEmergency)
                    .ThenBy(t => t.PrintTime)
                    .ToListAsync();
            }
        }

        // If pooling is disabled or service point not found, return tickets for this specific service point
        return await context.QueuedTickets
            .Where(t => t.ServicePointId == servicePointId)
            .OrderByDescending(t => t.IsEmergency)
            .ThenBy(t => t.PrintTime)
            .ToListAsync();
    }


    public async Task<List<Ticket>> GetQueuedTicketsForServicePointAsync(int servicePointId)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        return await context.QueuedTickets
            .Where(t => t.ServicePointId == servicePointId
                     && (t.Status == TicketStatus.InQueue || t.Status == TicketStatus.Called))
            .OrderBy(t => t.PrintTime)
            .ToListAsync();
    }

    public async Task UpdateTicketStatusAsync(int id, string newStatus)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        var ticket = await context.QueuedTickets.FindAsync(id);

        if (ticket == null)
        {
            throw new ArgumentException("Ticket not found", nameof(id));
        }

        var oldStatus = ticket.Status;
        ticket.Status = newStatus;

        if (newStatus == TicketStatus.NoShow)
        {
            NoShowTickets.Add(ticket);
        }

        await context.SaveChangesAsync();

        if (oldStatus == TicketStatus.Called)
        {
            TicketRemovedFromCalledEvent?.Invoke(this, ticket.TicketNumber);
        }

        TicketQueueAlteredEvent?.Invoke(this, EventArgs.Empty);
    }

    public async Task UpdateServicePointStatusAsync(int servicePointId, string status)
    {
        using var context = _dbFactory.CreateDbContext();
        var servicePoint = await context.ServicePoints.FindAsync(servicePointId);
        if (servicePoint != null)
        {
            servicePoint.Status = status;
            await context.SaveChangesAsync();
        }
    }

    public async Task RemoveTicketFromQueueAsync(Ticket ticket)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        context.QueuedTickets.Remove(ticket);
        await context.SaveChangesAsync();

        TicketQueueAlteredEvent?.Invoke(this, EventArgs.Empty);
    }


    public async Task SaveServedTicketAsync(ServedTicket servedTicket)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        await context.ServedTickets.AddAsync(servedTicket);
        await context.SaveChangesAsync();
    }

    public async Task<ServicePoint> FindBestServicePointAsync(string serviceName)
    {
        using var context = _dbFactory.CreateDbContext();

        bool isPoolingEnabled = await IsPoolingEnabledAsync();

        if (isPoolingEnabled)
        {
            // If pooling is enabled, just return any active service point
            return await context.ServicePoints
                .Where(sp => sp.Service.Name == serviceName && sp.Active)
                .FirstOrDefaultAsync();
        }


        var queueToAvailableOnlyConfig = await GetConfigurationAsync("QueueToRoomWithAvailableUsersOnly");
        bool queueToAvailableOnly = queueToAvailableOnlyConfig?.BoolValue ?? false;

        var servicePoints = await context.ServicePoints
            .Where(sp => sp.Service.Name == serviceName && sp.Active)
            .Include(sp => sp.Service)
            .ToListAsync();

        if (!servicePoints.Any())
        {
            throw new InvalidOperationException($"No active service points found for service: {serviceName}");
        }

        // If there's only one service point, return it regardless of status
        if (servicePoints.Count == 1)
        {
            return servicePoints[0];
        }

        // Filter available service points based on configuration
        var availableServicePoints = queueToAvailableOnly
            ? servicePoints.Where(sp => sp.Status != "Stepped out").ToList()
            : servicePoints;

        // If no available service points and strict queueing is enabled, throw exception
        if (!availableServicePoints.Any() && queueToAvailableOnly)
        {
            throw new InvalidOperationException($"No available service points found for service: {serviceName}");
        }

        // If no available service points and strict queueing is disabled, fall back to all service points
        if (!availableServicePoints.Any())
        {
            availableServicePoints = servicePoints;
        }

        // Find the service point with the shortest queue
        var servicePointWithShortestQueue = availableServicePoints
            .OrderBy(sp => context.QueuedTickets.Count(t => t.ServicePointId == sp.Id && t.Status == TicketStatus.InQueue))
            .First();

        return servicePointWithShortestQueue;
    }

    public async Task<bool> IsPoolingEnabledAsync()
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        var poolingConfig = await context.Configurations
            .FirstOrDefaultAsync(c => c.ConfigurationName == "IsPoolingEnabled");
        return poolingConfig?.BoolValue ?? false;
    }

    public async Task<bool> LockTicketAsync(int ticketId, string serviceProviderId)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        var ticket = await context.QueuedTickets.FindAsync(ticketId);

        if (ticket == null || ticket.IsLocked)
            return false;

        ticket.IsLocked = true;
        ticket.LockedByServiceProviderId = serviceProviderId;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task UnlockTicketAsync(int ticketId)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        var ticket = await context.QueuedTickets.FindAsync(ticketId);

        if (ticket != null)
        {
            ticket.IsLocked = false;
            ticket.LockedByServiceProviderId = null;
            await context.SaveChangesAsync();
        }
    }

    public async Task<Ticket> GenerateTicketAsync(string serviceName, string customerName,
        string customerPhoneNumber, string IdNumber, bool isEmergency = false, bool isLocked = false)
    {
        var bestServicePoint = await FindBestServicePointAsync(serviceName);
        string ticketNumber = await GenerateTicketNumberAsync();

        var ticket = new Ticket
        {
            TicketNumber = ticketNumber,
            ServiceName = serviceName,
            PrintTime = DateTime.Now,
            Status = TicketStatus.InQueue,
            CustomerName = customerName,
            CustomerPhoneNumber = customerPhoneNumber,
            IdNumber = IdNumber,
            ServicePointId = bestServicePoint.Id,
            IsEmergency = isEmergency,
            ServicePointAssignmentTime = DateTime.Now,
            IsLocked = isLocked,
            LockedByServiceProviderId = null,
        };

        await AddTicketToQueueAsync(ticket);
        return ticket;
    }

    private async Task AddTicketToQueueAsync(Ticket ticket)
    {
        using var context = _dbFactory.CreateDbContext();
        context.QueuedTickets.Add(ticket);
        await context.SaveChangesAsync();

        TicketQueueAlteredEvent?.Invoke(this, EventArgs.Empty);
    }

    public async Task<int> GetCustomersAheadCountAsync(int ticketId)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        var ticket = await context.QueuedTickets
            .Where(t => t.Id == ticketId)
            .Select(t => new { t.Id, t.ServicePointId, t.PrintTime })
            .FirstOrDefaultAsync();

        if (ticket == null)
        {
            return -1; // Indicates ticket not found
        }

        return await context.QueuedTickets
            .CountAsync(t => t.ServicePointId == ticket.ServicePointId
                          && t.PrintTime < ticket.PrintTime
                          && (t.Status == TicketStatus.InQueue || t.Status == TicketStatus.Called));
    }

    public async Task<TimeSpan> EstimateWaitingTimeAsync(int ticketId)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        var ticket = await context.QueuedTickets
            .Where(t => t.Id == ticketId)
            .Select(t => new { t.Id, t.ServicePointId, t.PrintTime })
            .FirstOrDefaultAsync();

        if (ticket == null)
        {
            return TimeSpan.Zero; // Or throw an exception if preferred
        }

        var averageServiceTime = await GetAverageServiceTimeAsync(ticket.ServicePointId);

        var ticketsAhead = await context.QueuedTickets
            .CountAsync(t => t.ServicePointId == ticket.ServicePointId
                          && t.PrintTime < ticket.PrintTime
                          && t.Status == TicketStatus.InQueue);

        return averageServiceTime * ticketsAhead;
    }

    public async Task<TimeSpan> GetAverageServiceTimeAsync(int servicePointId)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        var recentTickets = await context.ServedTickets
            .Where(st => st.ServicePointId == servicePointId)
            .OrderByDescending(st => st.FinishTime)
            .Take(2)
            .ToListAsync();

        if (!recentTickets.Any())
        {
            return TimeSpan.FromMinutes(10); // Default to 10 minutes if no data
        }

        var averageServiceTime = recentTickets
            .Select(st => (st.FinishTime - st.ShowTime).TotalMinutes)
            .Average();

        return TimeSpan.FromMinutes(averageServiceTime);
    }

    public async Task<List<Service>> GetAvailableServicesAsync()
    {
        // Logic to retrieve available services (e.g., from a database)
        await using var context = await _dbFactory.CreateDbContextAsync();
        return await context.Services.Where(s => s.Active).ToListAsync();
    }

    private async Task<Configuration?> GetConfigurationAsync(string configName)
    {
        using var context = await _dbFactory.CreateDbContextAsync();
        return await context.Configurations
            .FirstOrDefaultAsync(c => c.ConfigurationName == configName);
    }

    //private void InitializeQueueCleanupTimer()
    //{
    //    // Check every hour for tickets that need to be removed
    //    _queueCleanupTimer = new Timer(async _ => await CleanupExpiredTickets(), null, TimeSpan.Zero, TimeSpan.FromHours(1));
    //}

    public async Task CleanupExpiredTickets()
    {
        try
        {
            using var context = await _dbFactory.CreateDbContextAsync();
            var removeAfterHrsConfig = await GetConfigurationAsync("RemovePatientsFromQueueAfterHrs");
            if (removeAfterHrsConfig?.IntValue == null || removeAfterHrsConfig.IntValue == 0)
            {
                _logger.LogInformation("Ticket cleanup is disabled or not configured");
                return;
            }

            var hours = removeAfterHrsConfig.IntValue;
            var cutoffTime = DateTime.Now.AddHours((double)-hours);
            var expiredTickets = await context.QueuedTickets
                .Where(t => t.PrintTime < cutoffTime &&
                           (t.Status == TicketStatus.InQueue || t.Status == TicketStatus.Called))
                .ToListAsync();

            if (expiredTickets.Any())
            {
                _logger.LogInformation("Removing {Count} expired tickets from queue", expiredTickets.Count);
                context.QueuedTickets.RemoveRange(expiredTickets);
                await context.SaveChangesAsync();
                TicketQueueAlteredEvent?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while cleaning up expired tickets");
            throw;
        }
    }

    public async Task ResetQueueAsync()
    {
        using var context = await _dbFactory.CreateDbContextAsync();

        // Get all tickets that are in queue, called or showed up
        var activeTickets = await context.QueuedTickets
            .Where(t => t.Status != TicketStatus.InService)
            .ToListAsync();

        // Remove all active tickets
        context.QueuedTickets.RemoveRange(activeTickets);

        // Clear the NoShowTickets list
        NoShowTickets.Clear();

        await context.SaveChangesAsync();

        // Notify subscribers that the queue has been altered
        TicketQueueAlteredEvent?.Invoke(this, EventArgs.Empty);
    }

    public async Task MarkTicketAsEmergencyAsync(int ticketId)
    {
        await using var context = await _dbFactory.CreateDbContextAsync();
        var ticket = await context.QueuedTickets
            .FirstOrDefaultAsync(t => t.Id == ticketId);

        if (ticket == null)
        {
            throw new ArgumentException("Ticket not found", nameof(ticketId));
        }

        ticket.IsEmergency = true;
        await context.SaveChangesAsync();

        // Notify subscribers that the queue has been altered
        TicketQueueAlteredEvent?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? TicketQueueAlteredEvent;
    public event EventHandler<(string, string)>? TicketCalledFromQueueEvent;
    public event EventHandler<string>? TicketRemovedFromCalledEvent;

}