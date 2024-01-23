public interface IEnemyState {
    public void Start();
    public void FixedUpdate();
    public void Update();
    public void CheckTransitions();
    public void Exit();
}