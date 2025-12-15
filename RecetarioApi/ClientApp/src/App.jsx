import React, { useEffect, useState } from 'react';
import './App.css';

const apiBase = '/api/Recetas'; // proxy de Vite redirige estas llamadas al backend

export default function App() {
    const [recipes, setRecipes] = useState([]);
    const [titulo, setTitulo] = useState('');
    const [ingredientes, setIngredientes] = useState('');
    const [pasos, setPasos] = useState('');
    const [categoria, setCategoria] = useState('');
    const [favorito, setFavorito] = useState(false);
    const [qIngrediente, setQIngrediente] = useState('');
    const [qCategoria, setQCategoria] = useState('');

    async function fetchRecipes() {
        const qs = new URLSearchParams();
        if (qIngrediente) qs.set('ingrediente', qIngrediente);
        if (qCategoria) qs.set('categoria', qCategoria);
        const res = await fetch(`${apiBase}?${qs.toString()}`);
        const data = await res.json();
        setRecipes(data);
    }

    useEffect(() => { fetchRecipes(); }, []);

    async function handleCreate(e) {
        e.preventDefault();
        const body = {
            titulo,
            ingredientes: ingredientes.split(',').map(s => s.trim()).filter(Boolean),
            pasos: pasos.split(';').map(s => s.trim()).filter(Boolean),
            categoria,
            favorito
        };
        const res = await fetch(apiBase, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body)
        });
        if (res.ok) {
            setTitulo(''); setIngredientes(''); setPasos(''); setCategoria(''); setFavorito(false);
            fetchRecipes();
        } else {
            const err = await res.json().catch(() => null);
            alert('Error al crear receta' + (err?.title ? ': ' + err.title : ''));
        }
    }

    async function toggleFavorito(id, value) {
        const res = await fetch(`${apiBase}/${id}/favorito`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(value)
        });
        if (res.ok) fetchRecipes();
    }

    async function handleDelete(id) {
        if (!confirm('Eliminar receta?')) return;
        const res = await fetch(`${apiBase}/${id}`, { method: 'DELETE' });
        if (res.ok) fetchRecipes();
    }

    return (
        <div className="container py-4">
            <div className="app-header">
                <div>
                    <h1 className="app-title">Recetario</h1>
                    <div className="app-subtitle">Crea, filtra y marca tus recetas favoritas</div>
                </div>
            </div>

            <div className="card form-card mb-4">
                <div className="card-body">
                    <form onSubmit={handleCreate}>
                        <div className="row g-2">
                            <div className="col-md-6">
                                <input className="form-control" placeholder="Titulo" value={titulo} onChange={e => setTitulo(e.target.value)} required />
                            </div>
                            <div className="col-md-6">
                                <input className="form-control" placeholder="Categoria" value={categoria} onChange={e => setCategoria(e.target.value)} />
                            </div>
                            <div className="col-12">
                                <input className="form-control" placeholder="Ingredientes (separados por coma)" value={ingredientes} onChange={e => setIngredientes(e.target.value)} required />
                            </div>
                            <div className="col-12">
                                <input className="form-control" placeholder="Pasos (separar por ; )" value={pasos} onChange={e => setPasos(e.target.value)} required />
                            </div>
                            <div className="col-auto d-flex align-items-center">
                                <div className="form-check me-3">
                                    <input className="form-check-input" type="checkbox" checked={favorito} onChange={e => setFavorito(e.target.checked)} id="fav" />
                                    <label className="form-check-label" htmlFor="fav">Favorito</label>
                                </div>
                                <button className="btn btn-primary" type="submit">Crear</button>
                            </div>
                        </div>
                    </form>
                </div>
            </div>

            <div className="mb-3 d-flex gap-2 filter-bar">
                <input className="form-control" placeholder="Filtrar por ingrediente" value={qIngrediente} onChange={e => setQIngrediente(e.target.value)} />
                <input className="form-control" placeholder="Filtrar por categoria" value={qCategoria} onChange={e => setQCategoria(e.target.value)} />
                <button className="btn btn-outline-secondary" onClick={fetchRecipes}>Buscar</button>
            </div>

            <div className="row">
                {recipes.map(r => (
                    <div className="col-md-6" key={r.id}>
                        <div className="card recipe-card mb-3">
                            <div className="card-body">
                                <h5 className="card-title d-flex justify-content-between recipe-title">
                                    <span>{r.titulo}</span>
                                    <small className="text-muted">{r.categoria}</small>
                                </h5>
                                <p><strong>Ingredientes:</strong> {r.ingredientes.join(', ')}</p>
                                <p><strong>Pasos:</strong> <ol>{r.pasos.map((p, i) => <li key={i}>{p}</li>)}</ol></p>
                                <div className="d-flex justify-content-between">
                                    <div>
                                        <button
                                            className={`btn btn-sm btn-fav ${r.favorito ? 'active' : ''}`}
                                            onClick={() => toggleFavorito(r.id, !r.favorito)}
                                        >
                                            {r.favorito ? 'Quitar favorito' : 'Marcar favorito'}
                                        </button>
                                        <button className="btn btn-sm btn-danger ms-2" onClick={() => handleDelete(r.id)}>Eliminar</button>
                                    </div>
                                    <small className="text-muted">ID {r.id}</small>
                                </div>
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}