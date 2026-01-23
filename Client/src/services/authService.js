import axios from 'axios'

const API_URL = "/api/auth";

const TOKEN_KEY = 'userToken'
const ROLE_KEY = 'userRole'
const EMAIL_KEY = 'userEmail'

function decodeJwt(token) {
  try {
    const payload = token.split('.')[1]
    const json = atob(payload.replace(/-/g, '+').replace(/_/g, '/'))
    return JSON.parse(json)
  } catch {
    return null
  }
}

function getRoleFromDecoded(decoded) {
  const role =
    decoded?.role ||
    decoded?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']

  // kan string of array zijn
  if (Array.isArray(role)) return role[0] ?? 'User'
  return role ?? 'User'
}

function getEmailFromDecoded(decoded) {
  return (
    decoded?.unique_name ||
    decoded?.name ||
    decoded?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ||
    ''
  )
}

export async function login(username, password) {
  const res = await axios.post(`${API_URL}/login`, { username, password })
  const token = res.data?.token ?? res.data

  if (token) {
    localStorage.setItem(TOKEN_KEY, token)

    const decoded = decodeJwt(token)
    const role = getRoleFromDecoded(decoded)
    localStorage.setItem(ROLE_KEY, role)

    const email = getEmailFromDecoded(decoded)
    if (email) localStorage.setItem(EMAIL_KEY, email)
  }

  return res.data
}

export function logout() {
  localStorage.removeItem(TOKEN_KEY)
  localStorage.removeItem(ROLE_KEY)
  localStorage.removeItem(EMAIL_KEY)
}

export function getToken() {
  return localStorage.getItem(TOKEN_KEY)
}

export function isLoggedIn() {
  return !!getToken()
}

export function getRole() {
  return localStorage.getItem(ROLE_KEY) || 'User'
}

export function getEmail() {
  return localStorage.getItem(EMAIL_KEY) || ''
}

export function getUserRoleFromToken() {
  return getRole()
}
