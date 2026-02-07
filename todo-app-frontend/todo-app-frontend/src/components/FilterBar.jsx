import React from 'react';
import './FilterBar.css';

function FilterBar({ currentFilter, onFilterChange, activeTodoCount, hasCompletedTodos, onClearCompleted }) {
  return (
    <div className="filter-bar">
      <div className="filter-buttons">
        <button
          className={`filter-btn ${currentFilter === 'all' ? 'active' : ''}`}
          onClick={() => onFilterChange('all')}
        >
          All
        </button>
        <button
          className={`filter-btn ${currentFilter === 'active' ? 'active' : ''}`}
          onClick={() => onFilterChange('active')}
        >
          Active
        </button>
        <button
          className={`filter-btn ${currentFilter === 'completed' ? 'active' : ''}`}
          onClick={() => onFilterChange('completed')}
        >
          Completed
        </button>
      </div>

      <div className="filter-info">
        <span className="todo-count">
          {activeTodoCount} {activeTodoCount === 1 ? 'item' : 'items'} left
        </span>
        {hasCompletedTodos && (
          <button
            onClick={onClearCompleted}
            className="btn-clear"
          >
            Clear completed
          </button>
        )}
      </div>
    </div>
  );
}

export default FilterBar;
