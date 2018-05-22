using System;
using System.IO;
using System.Runtime.InteropServices;

namespace PinvokeFun
{
    class Program
    {
        public delegate void wkhtmltopdf_str_callback(IntPtr converter, string error);
        public delegate void wkhtmltopdf_void_callback(IntPtr converter);
        public delegate void wkhtmltopdf_int_callback(IntPtr converter, int val);

        [DllImport("wkhtmltox")]
        static extern int wkhtmltopdf_init(int use_graphics);

        [DllImport("wkhtmltox")]
        static extern IntPtr wkhtmltopdf_create_global_settings();

        [DllImport("wkhtmltox")]
        static extern int wkhtmltopdf_set_global_setting(IntPtr settings, string name, string value);

        [DllImport("wkhtmltox")]
        static extern IntPtr wkhtmltopdf_create_converter(IntPtr settings);

        [DllImport("wkhtmltox")]
        static extern IntPtr wkhtmltopdf_create_object_settings();

        [DllImport("wkhtmltox")]
        static extern IntPtr wkhtmltopdf_set_object_setting(IntPtr settings, string name, string value);

        [DllImport("wkhtmltox")]
        static extern IntPtr wkhtmltopdf_add_object(IntPtr converter, IntPtr settings, string data);

        [DllImport("wkhtmltox")]
        static extern IntPtr wkhtmltopdf_convert(IntPtr converter);

        [DllImport("wkhtmltox")]
        static extern void wkhtmltopdf_set_error_callback(IntPtr converter, wkhtmltopdf_str_callback callback);

        [DllImport("wkhtmltox")]
        static extern void wkhtmltopdf_set_phase_changed_callback(IntPtr converter, wkhtmltopdf_void_callback callback);

        [DllImport("wkhtmltox")]
        static extern void wkhtmltopdf_set_warning_callback(IntPtr converter, wkhtmltopdf_str_callback callback);

        [DllImport("wkhtmltox")]
        static extern void wkhtmltopdf_set_progress_changed_callback(IntPtr converter, wkhtmltopdf_int_callback callback);

        [DllImport("wkhtmltox")]
        static extern void wkhtmltopdf_set_finished_callback(IntPtr converter, wkhtmltopdf_int_callback callback);

        [DllImport("wkhtmltox")]
        static extern long wkhtmltopdf_get_output(IntPtr converter, IntPtr data);
        
        static void Main(string[] args)
        {
            if (wkhtmltopdf_init(0) == 1)
            {
                Console.WriteLine("wkhtmltopdf_init");

                IntPtr settings = wkhtmltopdf_create_global_settings();
                IntPtr converter = wkhtmltopdf_create_converter(settings);

                //wkhtmltopdf_set_global_setting(settings, "out", "out.pdf");

                IntPtr objectSettings = wkhtmltopdf_create_object_settings();
                wkhtmltopdf_set_object_setting(objectSettings, "page", @"D:\GitHub\DieKeure\HardcoverForm\OLF\Views\Home\webform\index.html");
                wkhtmltopdf_add_object(converter, objectSettings, null);

                wkhtmltopdf_str_callback onError = new wkhtmltopdf_str_callback(OnError);
                wkhtmltopdf_void_callback onPhaseChanged = new wkhtmltopdf_void_callback(OnPhaseChanged);
                wkhtmltopdf_str_callback onWarning = new wkhtmltopdf_str_callback(OnWarning);
                wkhtmltopdf_int_callback onProgressChanged = new wkhtmltopdf_int_callback(OnProgressChanged);
                wkhtmltopdf_int_callback onFinished = new wkhtmltopdf_int_callback(OnFinished);

                wkhtmltopdf_set_error_callback(converter, onError);
                wkhtmltopdf_set_phase_changed_callback(converter, onPhaseChanged);
                wkhtmltopdf_set_warning_callback(converter, onWarning);
                wkhtmltopdf_set_progress_changed_callback(converter, onProgressChanged);
                wkhtmltopdf_set_finished_callback(converter, onFinished);

                wkhtmltopdf_convert(converter);
            }
        }

        static void OnError(IntPtr converter, string error)
        {
            Console.WriteLine("Error: " + error);
        }

        static void OnPhaseChanged(IntPtr converter)
        {
            Console.WriteLine("Phase changed");
        }

        static void OnWarning(IntPtr converter, string warning)
        {
            Console.WriteLine("Warning: " + warning);
        }

        static void OnProgressChanged(IntPtr converter, int val)
        {
            Console.WriteLine("Progress: " + val);
        }

        static void OnFinished(IntPtr converter, int val)
        {
            Console.WriteLine("Finished: " + val);

            var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));
            try
            {
                long size = wkhtmltopdf_get_output(converter, ptr);

                byte[] pdfBytes = new byte[size];

                IntPtr deref1 = (IntPtr)Marshal.PtrToStructure(ptr, typeof(IntPtr));
                Marshal.Copy(deref1, pdfBytes, 0, pdfBytes.Length);

                File.WriteAllBytes("out.pdf", pdfBytes);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}
