import React, { useState, useEffect } from 'react';
import TodoList from './components/TodoList';
import AddTodo from './components/AddTodo';
import FilterBar from './components/FilterBar';
import todoApi from './services/todoApi';
import './App.css';

function App() {
  const [todos, setTodos] = useState([]);
  const [filter, setFilter] = useState('all'); // 'all', 'active', 'completed'
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Load todos from API on mount
  useEffect(() => {
    loadTodos();
  }, []);

  const loadTodos = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await todoApi.getAllTodos();
      setTodos(data);
    } catch (err) {
      setError('Failed to load todos. Make sure the API is running.');
      console.error('Error loading todos:', err);
    } finally {
      setLoading(false);
    }
  };

  const addTodo = async (title, description = '') => {
    try {
      const newTodo = await todoApi.createTodo(title, description);
      setTodos([newTodo, ...todos]);
    } catch (err) {
      setError('Failed to create todo');
      console.error('Error creating todo:', err);
    }
  };

  const toggleTodo = async (id) => {
    try {
      const updatedTodo = await todoApi.toggleTodo(id);
      setTodos(todos.map(todo =>
        todo.id === id ? updatedTodo : todo
      ));
    } catch (err) {
      setError('Failed to toggle todo');
      console.error('Error toggling todo:', err);
    }
  };

  const deleteTodo = async (id) => {
    try {
      await todoApi.deleteTodo(id);
      setTodos(todos.filter(todo => todo.id !== id));
    } catch (err) {
      setError('Failed to delete todo');
      console.error('Error deleting todo:', err);
    }
  };

  const updateTodo = async (id, updates) => {
    try {
      const updatedTodo = await todoApi.updateTodo(id, updates);
      setTodos(todos.map(todo =>
        todo.id === id ? updatedTodo : todo
      ));
    } catch (err) {
      setError('Failed to update todo');
      console.error('Error updating todo:', err);
    }
  };

  const clearCompleted = async () => {
    try {
      const completedTodos = todos.filter(todo => todo.completed);
      await Promise.all(completedTodos.map(todo => todoApi.deleteTodo(todo.id)));
      setTodos(todos.filter(todo => !todo.completed));
    } catch (err) {
      setError('Failed to clear completed todos');
      console.error('Error clearing completed todos:', err);
    }
  };

  // Filter todos based on current filter
  const filteredTodos = todos.filter(todo => {
    if (filter === 'active') return !todo.completed;
    if (filter === 'completed') return todo.completed;
    return true; // 'all'
  });

  const activeTodoCount = todos.filter(todo => !todo.completed).length;

  return (
    <div className="app">
      <header className="app-header">
        <h1>My Todo List</h1>
        <p className="subtitle">Stay organized, one task at a time</p>
      </header>

      <main className="app-main">
        {error && (
          <div className="error-banner">
            <p>{error}</p>
            <button onClick={loadTodos} className="btn-retry">
              Retry
            </button>
          </div>
        )}

        <AddTodo onAdd={addTodo} />
        
        <FilterBar
          currentFilter={filter}
          onFilterChange={setFilter}
          activeTodoCount={activeTodoCount}
          hasCompletedTodos={todos.some(todo => todo.completed)}
          onClearCompleted={clearCompleted}
        />

        {loading ? (
          <div className="loading-state">
            <div className="spinner"></div>
            <p>Loading todos...</p>
          </div>
        ) : (
          <>
            <TodoList
              todos={filteredTodos}
              onToggle={toggleTodo}
              onDelete={deleteTodo}
              onUpdate={updateTodo}
            />

            {todos.length === 0 && !loading && (
              <div className="empty-state">
                <p>No todos yet. Add one above to get started!</p>
              </div>
            )}
          </>
        )}
      </main>
    </div>
  );
}

export default App;
