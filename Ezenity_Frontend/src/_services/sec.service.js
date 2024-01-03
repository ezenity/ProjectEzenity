import { BehaviorSubject } from "rxjs";

import config from "config";
import { fetchWrapper } from "@/_helpers";

const secSubject = new BehaviorSubject(null);
const baseUrl = `${config.apiUrl}/sec`;

export const secService = {
  getAll,
  getById,
  create,
  update,
  delete: _delete,
  sec: secSubject.asObservable(),
  get secValue() {
    return secSubject.value;
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
  return fetchWrapper.put(`${baseUrl}/${id}`, params).then((sec) => {
      sec = { ...secSubject.value, ...sec };
      secSubject.next(sec);

      return sec;
  });
}

// prefixed with underscore because 'delete' is a reserved word in javascript
function _delete(id) {
  return fetchWrapper.delete(`${baseUrl}/${id}`);
}