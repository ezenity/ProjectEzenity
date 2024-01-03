import React, { useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Formik, Field, Form, ErrorMessage } from 'formik';
import * as Yup from 'yup';

import { skillService, alertService } from '@/_services';

function AddEditSkill({ match, history }) {
    const { id } = match.params;
    const isAdminMode = !id;
    const initialValues = {
        title: '',
        percentage: ''
    };

    const validationSchema = Yup.object().shape({
        title: Yup.string()
            .required('Title is required'),
        percentage: Yup.number()
            .required('Skill percentage is required')
    });

    function onSubmit(fields, { setStatus, setSubmitting }) {
        setStatus();
        if (isAdminMode) {
            createSkill(fields, setSubmitting);
        } else {
            updateSkill(id, fields, setSubmitting);
        }
    }

    function createSkill(fields, setSubmitting) {
        skillService.create(fields)
            .then(() => {
                alertService.success('skill added successfully', { keepAfterRouteChange: true });
                history.back('.');
            })
            .catch(error => {
                setSubmitting(false);
                alertService.error(error + " Testing");
            });
    }

    function updateSkill(id, fields, setSubmitting) {
        skillService.update(id, fields)
            .then(() => {
                alertService.success('Update successful', { keepAfterRouteChange: true });
                history.back();
            })
            .catch(error => {
                setSubmitting(false);
                alertService.error(error);
            });
    }

    return (
        <Formik initialValues={initialValues} validationSchema={validationSchema} onSubmit={onSubmit}>
            {({ errors, touched, isSubmitting, setFieldValue }) => {
                useEffect(() => {
                    if (!isAdminMode) {
                        // get skill and set form fields
                        skillService.getById(id).then(skill => {
                            const fields = ['title', 'percentage'];
                            fields.forEach(field => setFieldValue(field, skill[field], false));
                        });
                    }
                  }, []);
            
                return (
                    <Form>
                        <h1>{isAdminMode ? 'Add Skill' : 'Edit User'}</h1>
                        <div className="form-row">
                            <div className="form-group col-5">
                                <label>Title</label>
                                <Field name="title" type="text" className={'form-control' + (errors.title && touched.title ? ' is-invalid' : '')} />
                                <ErrorMessage name="title" component="div" className="invalid-feedback" />
                            </div>
                            <div className="form-group col-5">
                                <label>Percentage</label>
                                <Field name="percentage" type="number" className={'form-control' + (errors.percentage && touched.percentage ? ' is-invalid' : '')} />
                                <ErrorMessage name="percentage" component="div" className="invalid-feedback" />
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

export { AddEditSkill };