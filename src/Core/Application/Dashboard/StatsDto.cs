namespace Cleanception.Application.Dashboard;

public class StatsDto
{
    public int AllAppCustomerCount { get; set; }
    public int PendingAppCustomerCount { get; set; }
    public int ActiveAcctivateCustomerCount { get; set; }
    public int AssignedPickerJobs { get; set; }
    public int StartedPickerJobs { get; set; }
    public Dictionary<string, double>? AppCustomerByBranchChart { get; set; } = new Dictionary<string, double>();

    public int EmployeeCount { get; set; }
    public int ProductCount { get; set; }
    public int BrandCount { get; set; }
    public int UserCount { get; set; }
    public int RoleCount { get; set; }
    public List<ChartSeries> DataEnterBarChart { get; set; } = new();
    public Dictionary<string, double>? ProductByBrandTypePieChart { get; set; }
}

public class OrderStatsDto
{
    public int OrderCounts { get; set; }
    public Dictionary<string, int> OrdersByBranchChart { get; set; } = new Dictionary<string, int>();
}

public class ChartSeries
{
    public string? Name { get; set; }
    public double[]? Data { get; set; }
}