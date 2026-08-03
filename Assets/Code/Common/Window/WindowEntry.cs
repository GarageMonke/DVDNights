namespace Common
{
    public class WindowEntry
    {
        public Window Window;
        public bool OpenInContainer;

        public WindowEntry(Window window, bool openInContainer)
        {
            Window = window;
            OpenInContainer = openInContainer;
        }
    }
}