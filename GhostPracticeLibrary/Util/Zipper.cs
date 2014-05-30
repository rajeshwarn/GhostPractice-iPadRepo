using System;
using System.IO;
using System.IO.Compression;

namespace GhostPractice
{
	public class Zipper
	{
		public Zipper ()
		{
		}
		public static void Decompress (FileInfo fi)
		{
			// Get the stream of the source file.
			using (FileStream inFile = fi.OpenRead()) {
				// Get original file extension, for example
				// "doc" from report.doc.gz.
				string curFile = fi.FullName;
				string origName = curFile.Remove (curFile.Length - 
                		fi.Extension.Length);

				//Create the decompressed file.
				using (FileStream outFile = File.Create(origName)) {
					using (GZipStream Decompress = new GZipStream(inFile,
                            CompressionMode.Decompress)) {
						// Copy the decompression stream 
						// into the output file.
						Decompress.CopyTo (outFile);
            		    
						Console.WriteLine ("Decompressed: {0}", fi.Name);

					}
				}
			}
		}
	}
	//
		
}

