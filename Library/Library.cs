
namespace Qib.LIBRARY
{
    class Library
    {
        public LibraryElement[] Elements;
        public int Count { get => Elements.Length; }
        public LibraryElement this[int i] => Elements[i];

        public Library(int Count) {
            Elements = new LibraryElement[Count];
        }
        
        public float AspectOfElement(int i) {
            return (float)Elements[i].Width / Elements[i].Height;
        }

        public void Sort() {
            Random R = new();
            Elements = Elements.OrderBy(X => R.Next()).ToArray();
        }
    }
}
