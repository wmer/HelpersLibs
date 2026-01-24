namespace HelpersLib.Directories; 
public class DirectoryHelper {

    public static string CreateDirectoryInDesktop(string folderName) => CreateDirectory(folderName);

    public static string CreateDirectory(string folderName, string? path = null, bool local = false) {
        if (string.IsNullOrEmpty(path) && !local) {
            path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        if (local) {
            path = Environment.CurrentDirectory;
        }

        path = $"{path}\\{folderName}";

        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        return path;
    }

    public static void LimparDiretorio(string path, DateTime? untilTheDate = null) {
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        var dilesInPath = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Select(x => new FileInfo(x)).ToList();

        if (untilTheDate != null) {
            dilesInPath = dilesInPath.Where(x => x.CreationTime.Date < untilTheDate?.Date).ToList();
        }

        foreach (var file in dilesInPath) {
            try {
                File.Delete(file.FullName);
                Console.WriteLine($"{file.Name} Apagado...");
            } catch (Exception e) {
                Console.WriteLine("");
                Console.WriteLine($"Um erro aconteceu: {e.Message}");
            }
        }
    }

}