public static class PropertyCopier<TParent, TChild> where TParent : class
                                            where TChild : class
{
    public static void Copy(TParent source, TChild destination)
    {
        var parentProperties = source.GetType().GetProperties();
        var childProperties = destination.GetType().GetProperties();

        foreach (var parentProperty in parentProperties)
        {
            foreach (var childProperty in childProperties)
            {
                if (parentProperty.Name == childProperty.Name && parentProperty.PropertyType == childProperty.PropertyType)
                {
                    childProperty.SetValue(destination, parentProperty.GetValue(source));
                    break;
                }
            }
        }
    }
}