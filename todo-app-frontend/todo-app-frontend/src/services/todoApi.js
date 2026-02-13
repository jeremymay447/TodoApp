const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

class TodoApiService {
  async getAllTodos() {
    const response = await fetch(`${API_BASE_URL}/todos`, {
    headers: this.getAuthHeader(),
  });
  
    if (response.status === 401) {
      this.logout();
      throw new Error('Session expired. Please login again.');
    }
    if (!response.ok) {
      throw new Error('Failed to fetch todos');
    }
    return response.json();
  }

  async getTodoById(id) {
    const response = await fetch(`${API_BASE_URL}/todos/${id}`, {
    headers: this.getAuthHeader(),
  });
    if (response.status === 401) {
      this.logout();
      throw new Error('Session expired. Please login again.');
    }
    if (!response.ok) {
      throw new Error('Failed to fetch todo');
    }
    return response.json();
  }

  async createTodo(title, description = '') {
    const response = await fetch(`${API_BASE_URL}/todos`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...this.getAuthHeader()
      },
      body: JSON.stringify({ title, description }),
    });
    
    if (response.status === 401) {
      this.logout();
      throw new Error('Session expired. Please login again.');
    }
    if (!response.ok) {
      throw new Error('Failed to create todo');
    }

    return response.json();
  }

  async updateTodo(id, updates) {
    const response = await fetch(`${API_BASE_URL}/todos/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        ...this.getAuthHeader
      },
      body: JSON.stringify(updates),
    });
    
    if (response.status === 401) {
      this.logout();
      throw new Error('Session expired. Please login again.');
    }
    if (!response.ok) {
      throw new Error('Failed to update todo');
    }

    return response.json();
  }

  async toggleTodo(id) {
    const response = await fetch(`${API_BASE_URL}/todos/${id}/toggle`, {
      method: 'PATCH', headers: this.getAuthHeader(),
    });
    
    if (response.status === 401) {
      this.logout();
      throw new Error('Session expired. Please login again.');
    }
    if (!response.ok) {
      throw new Error('Failed to toggle todo');
    }

    return response.json();
  }

  async deleteTodo(id) {
    const response = await fetch(`${API_BASE_URL}/todos/${id}`, {
      method: 'DELETE', headers: this.getAuthHeader(),
    });
    
    if (response.status === 401) {
      this.logout();
      throw new Error('Session expired. Please login again.');
    }
    if (!response.ok) {
      throw new Error('Failed to delete todo');
    }
  }

  getAuthHeader() {
    const token = localStorage.getItem('authToken');
    return token ? { 'Authorization': `Bearer ${token}` } : {};
  }

  async register(username, email, password) {
    const response = await fetch(`${API_BASE_URL}/auth/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, email, password }),
    });
  
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Registration failed');
    }
  
    const data = await response.json();
    localStorage.setItem('authToken', data.token);
    localStorage.setItem('user', JSON.stringify({
      userId: data.userId,
      username: data.username,
      email: data.email
    }));
    return data;
  }

  async login(username, password) {
    const response = await fetch(`${API_BASE_URL}/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password }),
    });
    
    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.message || 'Login failed');
    }
    
    const data = await response.json();
    localStorage.setItem('authToken', data.token);
    localStorage.setItem('user', JSON.stringify({
      userId: data.userId,
      username: data.username,
      email: data.email
    }));
    return data;
  }

  logout() {
    localStorage.removeItem('authToken');
    localStorage.removeItem('user');
  }

  getCurrentUser() {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  }

  isAuthenticated() {
    return !!localStorage.getItem('authToken');
  }
}

export default new TodoApiService();
