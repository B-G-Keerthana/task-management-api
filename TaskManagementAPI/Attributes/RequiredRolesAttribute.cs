namespace TaskManagementAPI.Attributes
{
    // This attribute can be applied to controllers or actions to specify required roles.
    // It supports Admin and User roles
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class RequiredRolesAttribute : Attribute
    {
        // Stores the list of roles required to access the decorated resource
        public string[] Roles { get; }

        // Constructor that accepts one or more roles
        public RequiredRolesAttribute(params string[] roles)
        {
            Roles = roles;
        }
    }
}
