import React, { useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Formik, Field, Form, ErrorMessage, FieldArray } from 'formik';
import * as Yup from 'yup';

import { phService, alertService } from '@/_services';

function AddEditProjectHistory({ match, history }) {
    const { id } = match.params;
    const isAdminMode = !id;
    const initialValues = {
        title: '',
        date: '',
        location: '',
        descriptions: []
    };

    const validationSchema = Yup.object().shape({
        title: Yup.string()
            .required('Title is required'),
        date: Yup.string()
            .required('Date is required'),
        location: Yup.string()
            .required('Location is required'),
        descriptions: Yup.array().of(
            Yup.object().shape({
                descriptionList: Yup.string().min(4, "Must contain at least 4 characters").required("Required")
            })
        )
    });

    function onSubmit(fields, { setStatus, setSubmitting }) {
        setStatus();
        if (isAdminMode) {
            createPH(fields, setSubmitting);
        } else {
            updatePH(id, fields, setSubmitting);
        }
    }

    function createPH(fields, setSubmitting) {
        phService.create(fields)
            .then(() => {
                alertService.success('Added successfully', { keepAfterRouteChange: true });
                history.back('.');
            })
            .catch(error => {
                setSubmitting(false);
                alertService.error(error + " Testing");
            });
    }

    function updatePH(id, fields, setSubmitting) {
        phService.update(id, fields)
            .then(() => {
                alertService.success('Updated successfully', { keepAfterRouteChange: true });
                history.back();
            })
            .catch(error => {
                setSubmitting(false);
                alertService.error(error);
            });
    }

    return (
        <Formik initialValues={initialValues} validationSchema={validationSchema} onSubmit={onSubmit}>
            {({ errors, touched, isSubmitting, values, setFieldValues }) => {
                useEffect(() => {
                    if (!isAdminMode) {
                        // get PJ and set form fields
                        phService.getById(id).then(ph => {
                            const fields = ['title', 'date', 'location', 'descriptions'];
                            fields.forEach(field => setFieldValues(field, ph[field], false));
                        });
                    }
                  }, []);
            
                return (
                    <Form>
                        <h1>{isAdminMode ? 'Add New' : 'Edit Content'}</h1>
                        <div className="form-row">
                            <div className="form-group col-5">
                                <label>Title</label>
                                <Field name="title" type="text" className={'form-control' + (errors.title && touched.title ? ' is-invalid' : '')} />
                                <ErrorMessage name="title" component="div" className="invalid-feedback" />
                            </div>
                            <div className="form-group col-5">
                                <label>Date</label>
                                <Field name="date" type="text" className={'form-control' + (errors.title && touched.title ? ' is-invalid' : '')} />
                                <ErrorMessage name="date" component="div" className="invalid-feedback" />
                            </div>
                            <div className="form-group col-5">
                                <label>Location</label>
                                <Field name="location" type="textarea" className={'form-control' + (errors.content && touched.content ? ' is-invalid' : '')} />
                                <ErrorMessage name="location" component="div" className="invalid-feedback" />
                            </div>
                            <div className="form-group col-5">
                                <label>Description List</label>
                                <FieldArray name="descriptions" render={arrayHelpers => (
                                    <div>
                                        {values.descriptions.map((description, i) => (
                                            <div key={i}>
                                                <Field name={`descriptions.${i}.descriptionList`} type="textarea" className={'form-control' + (errors.descriptions && touched.descriptions ? ' is-invalid' : '')} />
                                                <ErrorMessage name={`descriptions.${i}.descriptionList`} component="div" className="invalid-feedback" />
                                                <button type="button" className="btn btn-sm btn-danger m-1 float-end" onClick={() => arrayHelpers.remove(i)}>
                                                    Remove
                                                </button>
                                            </div> 
                                        ))}

                                        <button type="button" className="btn btn-sm btn-success m-1 float-end" onClick={() => arrayHelpers.push({ descriptionList: '' })} >
                                            Add
                                        </button>
                                    </div>
                                )}
                                />
                            </div>
                        </div>
                        <div className="form-group mt-1">
                            <button type="submit" disabled={isSubmitting} className="btn btn-sm btn-primary me-1">
                                {isSubmitting && <span className="spinner-border spinner-border-sm mr-1"></span>}
                                Save
                            </button>
                            <Link to={isAdminMode ? '.' : '..'} className="btn btn-sm btn-danger">Cancel</Link>
                        </div>
                    </Form>
                );
            }}
        </Formik>
    );
}

export { AddEditProjectHistory };