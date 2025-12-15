import { useEffect, useMemo, useState } from "react";

const emptyForm = {
  firstName: "",
  lastName: "",
  email: "",
  documentNumber: "",
  birthDate: "",
  role: "Employee",
  managerId: "",
  password: "",
  confirmPassword: "",
  newPassword: "",
};

export default function EmployeeForm({ onSubmit, onCancel, editing, employees, roles }) {
  const [form, setForm] = useState(emptyForm);
  const [phones, setPhones] = useState([{ label: "Principal", number: "" }]);

  useEffect(() => {
    if (editing) {
      setForm({
        firstName: editing.firstName,
        lastName: editing.lastName,
        email: editing.email,
        documentNumber: editing.documentNumber,
        birthDate: editing.birthDate?.slice(0, 10),
        role: editing.role,
        managerId: editing.managerId || "",
        password: "",
        confirmPassword: "",
        newPassword: "",
      });
      setPhones(
        editing.phones?.map((p) => ({ label: p.label, number: p.number })) ?? [
          { label: "Principal", number: "" },
        ]
      );
    } else {
      setForm(emptyForm);
      setPhones([{ label: "Principal", number: "" }]);
    }
  }, [editing]);

  const managerOptions = useMemo(
    () => employees.filter((e) => e.id !== editing?.id),
    [employees, editing]
  );

  const handleChange = (key, value) => {
    setForm((prev) => ({ ...prev, [key]: value }));
  };

  const handlePhoneChange = (index, key, value) => {
    setPhones((prev) => {
      const clone = [...prev];
      clone[index] = { ...clone[index], [key]: value };
      return clone;
    });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    const payload = {
      firstName: form.firstName,
      lastName: form.lastName,
      email: form.email,
      documentNumber: form.documentNumber,
      birthDate: form.birthDate,
      role: form.role,
      managerId: form.managerId || null,
      phones,
    };

    if (editing) {
      payload.newPassword = form.newPassword || null;
      onSubmit(payload, true);
    } else {
      payload.password = form.password;
      payload.confirmPassword = form.confirmPassword;
      onSubmit(payload, false);
    }
  };

  return (
    <div className="card">
      <div className="header">
        <div>
          <div className="badge">{editing ? "Edição" : "Cadastro"}</div>
          <h2>{editing ? "Editar colaborador" : "Novo colaborador"}</h2>
        </div>
        {editing && (
          <button
            className="button secondary"
            type="button"
            onClick={() => {
              setForm(emptyForm);
              setPhones([{ label: "Principal", number: "" }]);
              onCancel?.();
            }}
          >
            Cancelar edição
          </button>
        )}
      </div>

      <form onSubmit={handleSubmit} className="grid">
        <div className="field">
          <label>Primeiro nome</label>
          <input
            value={form.firstName}
            onChange={(e) => handleChange("firstName", e.target.value)}
            required
          />
        </div>
        <div className="field">
          <label>Sobrenome</label>
          <input
            value={form.lastName}
            onChange={(e) => handleChange("lastName", e.target.value)}
            required
          />
        </div>
        <div className="field">
          <label>E-mail</label>
          <input
            type="email"
            value={form.email}
            onChange={(e) => handleChange("email", e.target.value)}
            required
          />
        </div>
        <div className="field">
          <label>Documento</label>
          <input
            value={form.documentNumber}
            onChange={(e) => handleChange("documentNumber", e.target.value)}
            required
          />
        </div>
        <div className="field">
          <label>Data de nascimento</label>
          <input
            type="date"
            value={form.birthDate}
            onChange={(e) => handleChange("birthDate", e.target.value)}
            required
          />
        </div>
        <div className="field">
          <label>Perfil</label>
          <select
            value={form.role}
            onChange={(e) => handleChange("role", e.target.value)}
          >
            {roles.map((role) => (
              <option key={role} value={role}>
                {role}
              </option>
            ))}
          </select>
        </div>
        <div className="field">
          <label>Gestor</label>
          <select
            value={form.managerId}
            onChange={(e) => handleChange("managerId", e.target.value)}
          >
            <option value="">Sem gestor</option>
            {managerOptions.map((m) => (
              <option key={m.id} value={m.id}>
                {m.firstName} {m.lastName} — {m.role}
              </option>
            ))}
          </select>
        </div>

        {phones.map((phone, idx) => (
          <div className="field" key={idx}>
            <label>Telefone #{idx + 1}</label>
            <div className="grid">
              <input
                placeholder="Etiqueta (ex: Principal)"
                value={phone.label}
                onChange={(e) => handlePhoneChange(idx, "label", e.target.value)}
                required
              />
              <input
                placeholder="Número"
                value={phone.number}
                onChange={(e) => handlePhoneChange(idx, "number", e.target.value)}
                required
              />
            </div>
          </div>
        ))}
        <button
          type="button"
          className="button secondary"
          onClick={() => setPhones((prev) => [...prev, { label: "Outro", number: "" }])}
        >
          Adicionar telefone
        </button>

        {!editing && (
          <>
            <div className="field">
              <label>Senha</label>
              <input
                type="password"
                value={form.password}
                onChange={(e) => handleChange("password", e.target.value)}
                required
              />
            </div>
            <div className="field">
              <label>Confirmar senha</label>
              <input
                type="password"
                value={form.confirmPassword}
                onChange={(e) => handleChange("confirmPassword", e.target.value)}
                required
              />
            </div>
          </>
        )}

        {editing && (
          <div className="field">
            <label>Nova senha (opcional)</label>
            <input
              type="password"
              value={form.newPassword}
              onChange={(e) => handleChange("newPassword", e.target.value)}
            />
          </div>
        )}

        <button type="submit" className="button">
          {editing ? "Salvar alterações" : "Criar colaborador"}
        </button>
      </form>
    </div>
  );
}
