using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ITVisions.Blazor;

public class FileManager
{
 public string relPathFilesDir = "";
 public string pathFilesDir = "";

 public int GetFileCount(string path, Guid guid)
 {
  var files = GetFiles(path, guid);
  if (files == null) return 0;
  return files.Where(f => !f.Name.EndsWith(".comment")).ToList().Count;
 }

 public List<FileInfo> GetFiles(string path, Guid? guid)
 {
  if (guid == null) return null;
  List<FileInfo> files = new List<FileInfo>();
  relPathFilesDir = Path.Combine("Files", guid.ToString());
  pathFilesDir = Path.Combine(path, relPathFilesDir);
  FileUtil.GetOrCreateDir(new DirectoryInfo(pathFilesDir));
  var di = new DirectoryInfo(pathFilesDir);
  if (di != null) files = di.GetFiles().ToList();
  return files;
 }

 public List<FileInfo> GetFiles(string pathFilesDir)
 {
  if (pathFilesDir == null) return null;
  List<FileInfo> files = new List<FileInfo>();
  FileUtil.GetOrCreateDir(new DirectoryInfo(pathFilesDir));
  var di = new DirectoryInfo(pathFilesDir);
  if (di != null) files = di.GetFiles().ToList();
  return files;
 }
}
