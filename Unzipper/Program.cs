using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
 
class Program
{
    static async Task Main(string[] args)
    {
#if DEBUG
        args = new[] { "-p", "H:\\Tera\\abc" };
#endif
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide a path.");
            return;
        }

        var pIndex = Array.IndexOf(args, "-p");
        if (pIndex == -1 || pIndex == args.Length - 1)
        {
            Console.WriteLine("Please provide a path with '-p' flag.");
            return;
        }

        var path = args[pIndex + 1];
        var processedFiles = new List<string>();
        await UnzipAndDeleteAsync(path, processedFiles);

        Console.WriteLine("Processing completed. Files processed:");
        foreach (var file in processedFiles)
        {
            Console.WriteLine($"\t{file}");
        }
    }

    static async Task UnzipAndDeleteAsync(string directoryPath, List<string> processedFiles)
    {
        var subdirectoryEntries = Directory.EnumerateDirectories(directoryPath);
        var tasks = subdirectoryEntries.Select(subdirectory => UnzipAndDeleteAsync(subdirectory, processedFiles));
        await Task.WhenAll(tasks);

        var zipFileEntries = Directory.EnumerateFiles(directoryPath, "*.zip");
        foreach (var zipFilePath in zipFileEntries)
        {
            await UnzipFileAsync(zipFilePath);
            processedFiles.Add(zipFilePath);
        }
    }

    static async Task UnzipFileAsync(string path)
    {
        await Task.Run(() =>
        {
            var directoryForUnzip = Path.GetDirectoryName(path);
            ZipFile.ExtractToDirectory(path, directoryForUnzip, true);
            File.Delete(path);
        });
    }
}