using System;
using System.Collections;
using System.Text;

namespace Engine
{
	class MessageQueue : Queue
	{
        public MessageQueue()
        {
            this.Clear();
        }


        public void AddItem(object socketEvent)
        {
            lock(this.SyncRoot)
            {
                this.Enqueue(socketEvent);
            }
        }

        public object DelItem()
        {
            lock(this.SyncRoot)
            {
                return (object)this.Dequeue();
            }
        }
        

        public object GetHeadItem()
        {
            lock(this.SyncRoot)
            {
                return this.Peek();
            }            
        }

        public void ClearAll()
        {
            lock (this.SyncRoot)
            {
                this.Clear();
            }
        }




    }
}
