namespace Gamegaard
{
    public class BoolAnimatorParameterHandler : AnimatorParameterSetter<bool>
    {
        public override void SetValue(bool value)
        {
            animator.SetBool(paramName, value);
        }

        public void Toggle()
        {
            animator.SetBool(paramName, !animator.GetBool(paramName));
        }
    }
}