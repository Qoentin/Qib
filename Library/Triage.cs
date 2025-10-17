using Qib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qib.LIBRARY
{
    public enum MediaType {
        Image,
        Video,
        Unknown
    }

    class Triage
    {
        public static readonly string[] ImageExtensions = { ".png", ".jpg", ".jpeg", ".jfif", ".gif" };
        public static readonly string[] VideoExtensions = { ".mp4" };

        public static IEnumerable<string> Images( string[] Files ) => Files.Where(X => X.HasFileExtension(ImageExtensions));

        public static IEnumerable<string> Images( string[] Files, out int Count ) { 
            var Triaged = Files.Where(X => X.HasFileExtension(ImageExtensions));
            Count = Triaged.Count();
            return Triaged;
        }

        public static IEnumerable<(MediaType, string)> Videos( string[] Files ) => Files.Select(X => X.HasFileExtension(VideoExtensions) ? (MediaType.Video, X) : (MediaType.Unknown, X)).Where(X => X.Item1 == MediaType.Video);

        public static IEnumerable<(MediaType, string)> Media( string[] Files ) =>
            Files.Select(X => {
                if ( X.HasFileExtension(ImageExtensions) ) return (MediaType.Image, X);
                else if ( X.HasFileExtension(VideoExtensions) ) return (MediaType.Video, X);
                else return (MediaType.Unknown, X);
            }).Where(X => X.Item1 != MediaType.Unknown);

    }
}
