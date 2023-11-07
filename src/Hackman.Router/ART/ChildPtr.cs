namespace Hackman.Router.ART
{
    public abstract class ChildPtr
    {
        public abstract Node Get();

        public abstract void Set(Node n);

        public virtual void Change(Node n)
        {
            // First increment the refcount of the new node, in case it would
            // otherwise have been deleted by the decrement of the old node
            n.refcount++;
            if (Get() != null)
            {
                Get().Decrement_refcount();
            }

            Set(n);
        }

        public virtual void Change_no_decrement(Node n)
        {
            n.refcount++;
            Set(n);
        }
    }
}