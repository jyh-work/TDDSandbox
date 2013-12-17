using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PyxisPrepAshp.Patterns
{
    public class WeakAction
    {
        private Action _staticAction;

        protected MethodInfo Method
        {
            get;
            set;
        }

        public virtual string MethodName
        {
            get
            {
                if (_staticAction != null)
                {
                    return _staticAction.Method.Name;
                }

                return Method.Name;
            }
        }

        protected WeakReference ActionReference
        {
            get;
            set;
        }

        protected WeakReference Reference
        {
            get;
            set;
        }

        public bool IsStatic
        {
            get
            {
                return _staticAction != null;
            }
        }

        protected WeakAction()
        {
        }

        public WeakAction(Action action)
            : this(action == null ? null : action.Target, action)
        {
        }

        public WeakAction(object target, Action action)
        {
            if (action.Method.IsStatic)
            {
                _staticAction = action;

                if (target != null)
                {
                    Reference = new WeakReference(target);
                }

                return;
            }

            Method = action.Method;

            ActionReference = new WeakReference(action.Target);

            Reference = new WeakReference(target);
        }

        /// <summary>
        /// Gets a value indicating whether the Action's owner is still alive, or if it was collected
        /// by the Garbage Collector already.
        /// </summary>
        public virtual bool IsAlive
        {
            get
            {
                if (_staticAction == null
                    && Reference == null)
                {
                    return false;
                }

                if (_staticAction != null)
                {
                    if (Reference != null)
                    {
                        return Reference.IsAlive;
                    }

                    return true;
                }

                return Reference.IsAlive;
            }
        }

        /// <summary>
        /// Gets the Action's owner. This object is stored as a 
        /// <see cref="WeakReference" />.
        /// </summary>
        public object Target
        {
            get
            {
                if (Reference == null)
                {
                    return null;
                }

                return Reference.Target;
            }
        }

        /// <summary>
        /// The target of the weak reference.
        /// </summary>
        protected object ActionTarget
        {
            get
            {
                if (ActionReference == null)
                {
                    return null;
                }

                return ActionReference.Target;
            }
        }

        /// <summary>
        /// Executes the action. This only happens if the action's owner
        /// is still alive.
        /// </summary>
        public void Execute()
        {
            if (_staticAction != null)
            {
                _staticAction();
                return;
            }

            var actionTarget = ActionTarget;

            if (IsAlive)
            {
                if (Method != null
                    && ActionReference != null
                    && actionTarget != null)
                {
                    Method.Invoke(actionTarget, null);
                    return;
                }
            }
        }

        /// <summary>
        /// Sets the reference that this instance stores to null.
        /// </summary>
        public void MarkForDeletion()
        {
            Reference = null;
            ActionReference = null;
            Method = null;
            _staticAction = null;
        }
    }
}
