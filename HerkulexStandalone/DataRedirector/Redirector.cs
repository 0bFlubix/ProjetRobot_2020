using System;
using EventArgsLibrary;


/// <summary>
/// Redirects sent data to an input event
/// </summary>


namespace DataRedirector
{
    public class Redirector
    {
        public void RedirectData(object sender, RedirectSentDataOutputArgs e)
        {
            OnDataRedirected(e.Data); 
        }

        //declare output event handler
        public event EventHandler<RedirectedDataArgs> OnRedirectedDataEvent;

        public virtual void OnDataRedirected(byte[] data)
        {
            var handler = OnRedirectedDataEvent;

            if(OnRedirectedDataEvent != null)
            {
                handler(this, new RedirectedDataArgs
                {   
                    Data = data
                });
            }
        }
    }
}
