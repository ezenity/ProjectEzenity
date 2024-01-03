import { BehaviorSubject } from "rxjs";

import config from "config";
import { fetchWrapper, history } from "@/_helpers";

const skillSubject = new BehaviorSubject(null);
const baseUrl = `${config.apiUrl}/Skills`;

export const skillService = {
  getAll,
  getById,
  create,
  update,
  delete: _delete,
  skill: skillSubject.asObservable(),
  get skillValue() {
    return skillSubject.value;
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
  return fetchWrapper.put(`${baseUrl}/${id}`, params).then((skill) => {
    // update stored skill if the logged skill updated their own record
    //if (skill.id === skillSubject.value.id) {
      // publish updated skill to subscribers
      skill = { ...skillSubject.value, ...skill };
      skillSubject.next(skill);
    //}
    return skill;
  });
}

// prefixed with underscore because 'delete' is a reserved word in javascript
function _delete(id) {
  return fetchWrapper.delete(`${baseUrl}/${id}`);

  // .then((x) => {
  //   // auto logout if the logged in user deleted their own record
  //   if (id === userSubject.value.id) {
  //     logout();
  //   }
  //   return x;
  // });
  
}