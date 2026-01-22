import api from './axiosInstance'

export async function getPatients() {
  const res = await api.get('admin/users/patients')
  return res.data
}

export async function getGeneralPractitioners() {
  const res = await api.get('admin/users/general-practitioners')
  return res.data
}

export async function getHospitalStaff() {
  const res = await api.get('admin/users/hospital-staff')
  return res.data
}

export async function createPatientAccount(payload) {
  const res = await api.post('admin/users/patient', payload)
  return res.data
}

export async function createGeneralPractitionerAccount(payload) {
  const res = await api.post('admin/users/general-practitioner', payload)
  return res.data
}

export async function createHospitalStaffAccount(payload) {
  const res = await api.post('admin/users/hospital-staff', payload)
  return res.data
}