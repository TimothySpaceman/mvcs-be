namespace Lib.Modules.Transfers.ConfigModels;

public record FtpStorageConfig(
    string Host,
    int Port,
    string Username,
    string? Password = null,
    
    bool Ssh = false,
    
    string? PrivateKey = null,
    string? PrivateKeyPassphrase = null,

    string FtpEncryption = "auto",
    bool FtpValidateCertificate = true,
    
    string RootPath = ""
);