export default function Layout({ user, onLogout, children }) {
  return (
    <div className="card">
      <div className="header">
        <div className="brand">
          <div className="badge">HR Manager</div>
          <div>
            <div className="muted">Gestão de colaboradores</div>
            <h2 style={{ margin: 0 }}>Olá, {user?.fullName}</h2>
          </div>
        </div>
        <div className="stack">
          <span className="pill">{user?.role}</span>
          <button className="button secondary" onClick={onLogout}>
            Sair
          </button>
        </div>
      </div>
      {children}
    </div>
  );
}
