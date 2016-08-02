using UnityEngine;
using System.Collections;
using System.IO;

public class FileFactory {

	static public void WriteBinary (byte[] data, string path) {
		ByteArrayToFile(path, data);
	}

	static bool ByteArrayToFile(string filePath, byte[] byteArray) {
	   try
	   {
	      FileStream _FileStream = 
	         new FileStream(filePath, FileMode.Create,
	                                  FileAccess.Write);
	      _FileStream.Write(byteArray, 0, byteArray.Length);
	      _FileStream.Close();
	      return true;
	   }
	   catch (IOException ex)
	   {
	      Logger.LogError("Exception caught in process: " + ex.ToString());
	   }

	   return false;
	}
}
