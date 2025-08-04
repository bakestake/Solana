using System.Text;

public class SaveSettings
{
    public string location;
    public string path;
    public string directory;
    public EncryptationType encryptationType;
    public string encryptationPassword;
    public int bufferSize;
    public Encoding encoding;

    public SaveSettings(string location = "", string path = "", string directory = "", EncryptationType encryptationType = EncryptationType.None, string encryptationPassword = "", int bufferSize = 0)
    {
        this.location = location;
        this.path = path;
        this.directory = directory;
        this.encryptationType = encryptationType;
        this.encryptationPassword = encryptationPassword;
        this.bufferSize = bufferSize;
    }

    public SaveSettings(Encoding encoding, string location = "", string path = "", string directory = "", EncryptationType encryptationType = EncryptationType.None, string encryptationPassword = "", int bufferSize = 0)
    {
        this.encoding = encoding;
        this.location = location;
        this.path = path;
        this.directory = directory;
        this.encryptationType = encryptationType;
        this.encryptationPassword = encryptationPassword;
        this.bufferSize = bufferSize;
    }
}
