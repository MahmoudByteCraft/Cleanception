namespace Cleanception.Shared.Constants;
public static class ConfirmationByConstants
{
    public static string Email = "Email";
    public static string Mobile = "Mobile";
}

public static class RoleConstants
{
    public static string B2BCustomer = "B2B Customer";
    public static string B2CCustomer = "B2C Customer";
    public static string WarehouseManager = "Warehouse Manager";
    public static string SalesManager = "Sales Manager";
    public static string Picker = "Picker";
    public static string CreditManager = "Credit Manager";
    public static string SalesPerson = "Sales Person";

    public static string? GetRoleKeyValue(string roleKey)
    {
        if (roleKey.ToLower() == "b2bcustomer")
            return B2BCustomer;

        if (roleKey.ToLower() == "b2ccustomer")
            return B2CCustomer;

        if (roleKey.ToLower() == "warehousemanager")
            return WarehouseManager;

        if (roleKey.ToLower() == "salesmanager")
            return SalesManager;

        if (roleKey.ToLower() == "picker")
            return Picker;

        if (roleKey.ToLower() == "creditmanager")
            return CreditManager;

        if (roleKey.ToLower() == "salesperson")
            return SalesPerson;

        return null;
    }
}

public static class NotificationVariablesNameConstants
{
    public static class NewUser
    {
        public const string FullName = "[Name]";

        public const string ConfirmationCode = "[Code]";
    }
}

public static class CacheConstants
{
    public const string User = "USER";
}