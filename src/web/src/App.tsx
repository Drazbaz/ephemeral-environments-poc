import { useEffect, useState, type SubmitEvent } from 'react'
import './App.css'

type User = {
  id: string
  name: string
}

type UserPage = {
  items: User[]
  hasMore: boolean
}

const pageSize = 5

async function fetchUsers(offset: number): Promise<UserPage> {
  const response = await fetch(`/api/user/?offset=${offset}&limit=${pageSize}`)
  if (!response.ok) {
    throw new Error(`Unable to load users (${response.status}).`)
  }

  return (await response.json()) as UserPage
}

function App() {
  const [users, setUsers] = useState<User[]>([])
  const [name, setName] = useState('')
  const [hasMore, setHasMore] = useState(false)
  const [loading, setLoading] = useState(true)
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState('')

  async function loadUsers(offset: number, append = false) {
    setLoading(true)
    setError('')

    try {
      const page = await fetchUsers(offset)
      setUsers((current) => append ? [...current, ...page.items] : page.items)
      setHasMore(page.hasMore)
    } catch (loadError) {
      setError(loadError instanceof Error ? loadError.message : 'Unable to load users.')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    let active = true

    void fetchUsers(0)
      .then((page) => {
        if (!active) return
        setUsers(page.items)
        setHasMore(page.hasMore)
      })
      .catch((loadError: unknown) => {
        if (active) {
          setError(loadError instanceof Error ? loadError.message : 'Unable to load users.')
        }
      })
      .finally(() => {
        if (active) setLoading(false)
      })

    return () => {
      active = false
    }
  }, [])

  async function handleCreateUser(event: SubmitEvent<HTMLFormElement>) {
    event.preventDefault()
    setSubmitting(true)
    setError('')

    try {
      const response = await fetch('/api/user/', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ name: name.trim() }),
      })

      if (!response.ok) {
        throw new Error(`Unable to create user (${response.status}).`)
      }

      setName('')
      await loadUsers(0)
    } catch (createError) {
      setError(createError instanceof Error ? createError.message : 'Unable to create user.')
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <main className="page">
      <header className="page-header">
        <p className="eyebrow">Directory</p>
        <h1>Users</h1>
      </header>

      <section className="create-section" aria-labelledby="create-heading">
        <h2 id="create-heading">Add a user</h2>
        <form className="create-form" onSubmit={handleCreateUser}>
          <label htmlFor="user-name">Name</label>
          <div className="form-controls">
            <input
              id="user-name"
              value={name}
              onChange={(event) => setName(event.target.value)}
              required
              maxLength={200}
              autoComplete="name"
            />
            <button type="submit" disabled={submitting || !name.trim()}>
              {submitting ? 'Adding…' : 'Add user'}
            </button>
          </div>
        </form>
      </section>

      <section className="users-section" aria-labelledby="users-heading">
        <div className="section-heading">
          <h2 id="users-heading">All users</h2>
          <span>{users.length} loaded</span>
        </div>

        {error && <p className="message error" role="alert">{error}</p>}
        {loading && users.length === 0 && <p className="message">Loading users…</p>}
        {!loading && users.length === 0 && !error && <p className="message">No users yet.</p>}

        {users.length > 0 && (
          <ul className="user-list">
            {users.map((user) => (
              <li key={user.id}>{user.name}</li>
            ))}
          </ul>
        )}

        {hasMore && (
          <button
            className="load-more"
            type="button"
            onClick={() => void loadUsers(users.length, true)}
            disabled={loading}
          >
            {loading ? 'Loading…' : 'Load more'}
          </button>
        )}
      </section>
    </main>
  )
}

export default App
