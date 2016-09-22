using System.IO;
using System.Runtime.InteropServices;

namespace LibEDF_DotNet
{
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
       Guid("29757a02-8a0c-47e2-96bb-2266c993c97a")]
    public interface IEDFFile
    {
        //Expose methods for COM use
        void Open(string edfFilePath);
        void Open(byte[] edfBytes);
        void Save(string edfFilePath);
    }

    [ClassInterface(ClassInterfaceType.None),
        Guid("07504667-1e49-4535-9c2f-157ee5b280b0")]
    public class EDFFile : IEDFFile
    {
        public EDFHeader Header { get; set; } = new EDFHeader();
        public EDFSignal[] Signals { get; set; }

        public EDFFile() { }
        public EDFFile(string edfFilePath) {
            Open(edfFilePath);
        }

        public EDFFile(byte[] edfBytes){
            Open(edfBytes);
        }

        public void ReadBase64(string edfBase64)
        {
            byte[] edfBytes = System.Convert.FromBase64String(edfBase64);
            Open(edfBytes);
        }

        public void Open(string edfFilePath)
        {
            using (var reader = new EDFReader(File.Open(edfFilePath, FileMode.Open)))
            {
                Header = reader.ReadHeader();
                Signals = reader.ReadSignals();
            }
        }

        public void Open(byte[] edfBytes)
        {
            using (var r = new EDFReader(edfBytes))
            {
                Header = r.ReadHeader();
                Signals = r.ReadSignals();
            }
        }

        public void Save(string edfFilePath)
        {
            if (Header == null) return;

            using (var writer = new EDFWriter(File.Open(edfFilePath, FileMode.Create)))
            {
                writer.WriteEDF(this, edfFilePath);
            }
        }
    }
}
