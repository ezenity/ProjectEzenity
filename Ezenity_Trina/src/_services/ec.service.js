import { BehaviorSubject } from "rxjs";

import config from "config";
import { fetchWrapper } from "@/_helpers";

const ecSubject = new BehaviorSubject(null);
const baseUrl = `${config.apiUrl}/ecs`;

export const ecService = {
  getAll,
  getById,
  create,
  update,
  delete: _delete,
  ec: ecSubject.asObservable(),
  get ecValue() {
    return ecSubject.value;
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
  return fetchWrapper.put(`${baseUrl}/${id}`, params).then((ec) => {
      ec = { ...ecSubject.value, ...ec };
      ecSubject.next(ec);

      return ec;
  });
}

// prefixed with underscore because 'delete' is a reserved word in javascript
function _delete(id) {
  return fetchWrapper.delete(`${baseUrl}/${id}`);
}