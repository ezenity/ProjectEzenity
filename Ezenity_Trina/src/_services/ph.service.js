import { BehaviorSubject } from "rxjs";

import config from "config";
import { fetchWrapper } from "@/_helpers";

const phSubject = new BehaviorSubject(null);
const baseUrl = `${config.apiUrl}/phs`;

export const phService = {
  getAll,
  getById,
  create,
  update,
  delete: _delete,
  ph: phSubject.asObservable(),
  get phValue() {
    return phSubject.value;
  },
};

function getAll() {
  return fetchWrapper.get(baseUrl);
}

function getById(id) {
  return fetchWrapper.get(`${baseUrl}/${id}`);
}

function create(params) {
  return fetchWrapper.post(baseUrl, params);
}

function update(id, params) {
  return fetchWrapper.put(`${baseUrl}/${id}`, params).then((ph) => {
      ph = { ...phSubject.value, ...ph };
      phSubject.next(ph);

      return ph;
  });
}

// prefixed with underscore because 'delete' is a reserved word in javascript
function _delete(id) {
  return fetchWrapper.delete(`${baseUrl}/${id}`);
}