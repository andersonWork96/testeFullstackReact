import { useState } from "react";

export default function LoginForm({ onSubmit, error }) {
  const [email, setEmail] = useState("admin@empresa.com");
  const [password, setPassword] = useState("Trocar@123");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      await onSubmit({ email, password });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="card">
      <div className="header">
        <div>
          <div className="badge">HR Manager</div>
          <h2>Entrar</h2>
          <p className="muted">Use o usuário seedado ou crie outro pela API.</p>
        </div>
      </div>
      <form onSubmit={handleSubmit} className="grid">
        <div className="field">
          <label>E-mail</label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            autoComplete="username"
          />
        </div>
        <div className="field">
          <label>Senha</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            autoComplete="current-password"
          />
        </div>
        <button className="button" type="submit" disabled={loading}>
          {loading ? "Autenticando..." : "Entrar"}
        </button>
      </form>
      {error && <p className="error">{error}</p>}
    </div>
  );
}
