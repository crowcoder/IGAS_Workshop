using System.Collections.Generic;

public class ProviderViewModel
{
    public string ProviderName { get; set; }
    public string Source {get;set;}
    public Dictionary<string,string> ConfigValues {get;set;}
}