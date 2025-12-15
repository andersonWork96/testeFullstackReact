import { useEffect, useMemo, useState } from "react";
import api from "./api/client";
import EmployeeForm from "./components/EmployeeForm.jsx";
import EmployeeTable from "./components/EmployeeTable.jsx";
import Layout from "./components/Layout.jsx";
import LoginForm from "./components/LoginForm.jsx";

const ROLE_ORDER = {
  Employee: 0,
  Leader: 1,
  Director: 2,
};

export default function App() {
  const [auth, setAuth] = useState(() => {
    const token = localStorage.getItem("token");
    const user = localStorage.getItem("user");
    return token && user ? { token, user: JSON.parse(user) } : null;
  });
  const [employees, setEmployees] = useState([]);
  const [editing, setEditing] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);

  const allowedRoles = useMemo(() => {
    if (!auth?.user?.role) return ["Employee"];
    return Object.keys(ROLE_ORDER).filter(
      (role) => ROLE_ORDER[role] <= ROLE_ORDER[auth.user.role]
    );
  }, [auth]);

  useEffect(() => {
    if (auth) {
      loadEmployees();
    }
  }, [auth]);

  const loadEmployees = async () => {
    setLoading(true);
    try {
      const { data } = await api.get("/api/employees");
      setEmployees(data);
      setError(null);
    } catch (err) {
      setError(readErrors(err));
    } finally {
      setLoading(false);
    }
  };

  const handleLogin = async (credentials) => {
    try {
      const { data } = await api.post("/api/auth/login", credentials);
      localStorage.setItem("token", data.token);
      localStorage.setItem("user", JSON.stringify(data.user));
      setAuth({ token: data.token, user: data.user });
      setError(null);
    } catch (err) {
      setError(readErrors(err));
    }
  };

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    setAuth(null);
    setEmployees([]);
    setEditing(null);
  };

  const handleSave = async (payload, isEditing) => {
    try {
      if (isEditing && editing) {
        await api.put(`/api/employees/${editing.id}`, payload);
      } else {
        await api.post("/api/employees", payload);
      }
      await loadEmployees();
      setEditing(null);
      setError(null);
    } catch (err) {
      setError(readErrors(err));
    }
  };

  const handleDelete = async (id) => {
    if (!confirm("Desativar colaborador?")) return;
    try {
      await api.delete(`/api/employees/${id}`);
      await loadEmployees();
    } catch (err) {
      setError(readErrors(err));
    }
  };

  if (!auth) {
    return (
      <div className="page">
        <LoginForm onSubmit={handleLogin} error={error} />
      </div>
    );
  }

  return (
    <div className="page">
      <Layout user={auth.user} onLogout={handleLogout}>
        {error && <p className="error">{error}</p>}
        {loading ? (
          <p className="muted">Carregando colaboradores...</p>
        ) : (
          <>
            <EmployeeForm
              onSubmit={handleSave}
              onCancel={() => setEditing(null)}
              editing={editing}
              employees={employees}
              roles={allowedRoles}
            />
            <h3 className="section-title">Equipe</h3>
            <EmployeeTable
              employees={employees}
              onEdit={setEditing}
              onDelete={handleDelete}
            />
          </>
        )}
      </Layout>
    </div>
  );
}

function readErrors(err) {
  const errors = err?.response?.data?.errors;
  if (Array.isArray(errors)) {
    return errors.join(", ");
  }
  return err?.response?.data?.message || "Algo deu errado. Tente novamente.";
}
