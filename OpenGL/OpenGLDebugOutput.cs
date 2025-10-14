using OpenTK.Graphics.OpenGL4;
using Qib.Extensions;
using System.Runtime.InteropServices;

namespace Qib.OPENGL
{
    class OpenGLDebugOutput
    {
        public string Title;
        private DebugProc Proc;

        public List<string> SupressIfContains = new();

        public OpenGLDebugOutput(string Title) {
            Proc = new DebugProc(
                ( DebugSource Source, DebugType Type, int ID, DebugSeverity Severity, int Length, IntPtr Message, IntPtr _ ) => {
                    ConsoleColor PreviousColor = SetConsoleColorBySeverity(Severity);


                    /*Console.WriteLine(Source == DebugSource.DebugSourceApplication ?
                        $"{Title} - {Marshal.PtrToStringAnsi(Message, Length)}" :
                        $"{Title} {GetSeverityIcon(Severity)}  {Type} (ID: {ID})\n{Marshal.PtrToStringAnsi(Message, Length)}\nFrom:{Source}"
                    );
                    
                    Console.WriteLine($"{Title} {GetSeverityIcon(Severity)}  {Type} (ID: {ID})\n{Marshal.PtrToStringAnsi(Message, Length)}\nFrom:{Source}");

                     */

                    string MsgString = Marshal.PtrToStringAnsi(Message, Length);

                    //if (MsgString.ContainsMultiple(SupressIfContains.ToArray()) ) {
                    //    return;
                    //}

                    Console.WriteLine($"{Title} {GetSeverityIcon(Severity)} \"{MsgString}\"");

                    if ( Severity == DebugSeverity.DebugSeverityHigh )
                        ;

                    Console.ForegroundColor = PreviousColor;
                }    
            );

            GL.DebugMessageCallback(Proc, IntPtr.Zero);
            GL.DebugMessageControl(DebugSourceControl.DontCare, DebugTypeControl.DontCare, DebugSeverityControl.DontCare, 0, new int[0], true);
            Insert("Debug output enabled!");
        }

        public void Insert(string Message, DebugSeverity Severity = DebugSeverity.DebugSeverityNotification) {
            GL.DebugMessageInsert(
                DebugSourceExternal.DebugSourceThirdParty,
                DebugType.DebugTypeOther,
                0,
                Severity,
                -1,
                Message
            );
        }

        private string GetSeverityIcon(DebugSeverity Severity) {
            return Severity switch {
                DebugSeverity.DebugSeverityLow => "(!)",
                DebugSeverity.DebugSeverityMedium => "(!!)",
                DebugSeverity.DebugSeverityHigh => "(!!!)",
                DebugSeverity.DebugSeverityNotification => "(I)",
                _ => "(?)"
            };
        }

        private ConsoleColor SetConsoleColorBySeverity(DebugSeverity Severity) {
            ConsoleColor Previous = Console.ForegroundColor;

            Console.ForegroundColor = Severity switch
            {
                DebugSeverity.DebugSeverityLow => ConsoleColor.Green,
                DebugSeverity.DebugSeverityMedium => ConsoleColor.Yellow,
                DebugSeverity.DebugSeverityHigh => ConsoleColor.Red,
                DebugSeverity.DebugSeverityNotification => ConsoleColor.Cyan,
                _ => Previous
            };

            return Previous;
        }
    }
}
