using System.Collections;

namespace AnimalManagement{
    public abstract class State
    {    
        protected Animal animal;

        public abstract void Tick();

        public virtual void OnStateEnter() { }
        public virtual void OnStateExit() { }

        public State(Animal animal)
        {
            this.animal = animal;
        }
    }
}