import api from "./axiosInstance";

export async function getMyPatients({ search = null, take = 50 } = {}) {
  const res = await api.get("huisarts/patienten", {
    params: { search, take }
  });
  return res.data;
}

export async function getReferralsForPatient(patientNumber) {
  const res = await api.get(`huisarts/patienten/${patientNumber}/referrals`);
  return res.data;
}

export async function createReferral(patientNumber, careCode) {
  const res = await api.post(`huisarts/patienten/${patientNumber}/referrals`, { careCode });
  return res.data;
}

export async function getGpTreatments() {
  const res = await api.get("huisarts/treatments");
  return res.data;
}
