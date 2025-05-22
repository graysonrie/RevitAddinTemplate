
namespace RealRevitPlugin.WpfWindow.Web.Core.WebEvents {
    public class State<T> {
        public T Value { get; set; }
        public static implicit operator T(State<T> state) => state.Value;

        public State(T value) {
            Value = value;
        }
    }

}
