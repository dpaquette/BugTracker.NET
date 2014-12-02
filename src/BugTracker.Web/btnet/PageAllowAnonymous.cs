using System;

namespace btnet
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class PageAllowAnonymous : Attribute
    {        
    }
}