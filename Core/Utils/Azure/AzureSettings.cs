namespace Core.Utils.Azure;

public class AzureSettings
{
    public string CdnDomain { get; set; } = null!;
    public string ConnectionStringStorageService { get; set; } = null!; 
    public string ConnectionStringMailService { get; set; } = null!;
    public string DefaultMailSenderAddress { get; set; } = null!;
}
