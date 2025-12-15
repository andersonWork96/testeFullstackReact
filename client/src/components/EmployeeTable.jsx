export default function EmployeeTable({ employees, onEdit, onDelete }) {
  if (!employees.length) {
    return <p className="muted">Nenhum colaborador cadastrado ainda.</p>;
  }

  return (
    <table className="table">
      <thead>
        <tr>
          <th>Nome</th>
          <th>E-mail</th>
          <th>Documento</th>
          <th>Perfil</th>
          <th>Gestor</th>
          <th>Ações</th>
        </tr>
      </thead>
      <tbody>
        {employees.map((emp) => (
          <tr key={emp.id}>
            <td>{emp.firstName} {emp.lastName}</td>
            <td>{emp.email}</td>
            <td>{emp.documentNumber}</td>
            <td><span className="pill">{emp.role}</span></td>
            <td>{emp.managerName || "-"}</td>
            <td className="stack">
              <button className="button secondary" type="button" onClick={() => onEdit(emp)}>Editar</button>
              <button className="button" type="button" onClick={() => onDelete(emp.id)}>Desativar</button>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}
