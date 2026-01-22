import api from './axiosInstance'

export async function getDoctors() {
  const res = await api.get('hospital/appointments/doctors')
  return res.data
}

export async function getPatients() {
  const res = await api.get('hospital/appointments/patients')
  return res.data
}

export async function getTreatments() {
  const res = await api.get('hospital/appointments/treatments')
  return res.data
}

export async function getRooms() {
  const res = await api.get('hospital/appointments/rooms')
  return res.data
}

export async function createAppointment(payload) {
  const res = await api.post('hospital/appointments/appointment', payload)
  return res.data
}

export async function getWeekSchedule({ weekStartIso, employeeNumber }) {
  const res = await api.get('hospital/appointments/schedule/week', {
    params: { weekStart: weekStartIso, employeeNumber }
  })
  return res.data
}

export async function cancelAppointment(id) {
  // bij jou is cancel = delete
  await api.post(`hospital/appointments/appointment/cancel/${id}`)
}
