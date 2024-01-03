import React, { useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Formik, Field, Form, ErrorMessage } from 'formik';
import * as Yup from 'yup';

import { ecService, alertService } from '@/_services';

function AddEditEC({ match, history }) {
    const { id } = match.params;
    const isAdminMode = !id;
    const initialValues = {
        title: '',
        content: ''
    };

    const validationSchema = Yup.object().shape({
        title: Yup.string()
            .required('Title is required'),
        content: Yup.string()
            .required('Content is required')
    });

    function onSubmit(fields, { setStatus, setSubmitting }) {
        setStatus();
        if (isAdminMode) {
            createEC(fields, setSubmitting);
        } else {
            updateEC(id, fields, setSubmitting);
        }
    }

    function createEC(fields, setSubmitting) {
        ecService.create(fields)
            .then(() => {
                alertService.success('Added successfully', { keepAfterRouteChange: true });
                history.back('.');
            })
            .catch(error => {
                setSubmitting(false);
                alertService.error(error + " Testing");
            });
    }

    function updateEC(id, fields, setSubmitting) {
        ecService.update(id, fields)
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
            {({ errors, touched, isSubmitting, setFieldValue }) => {
                useEffect(() => {
                    if (!isAdminMode) {
                        // get EC and set form fields
                        ecService.getById(id).then(ec => {
                            const fields = ['title', 'content'];
                            fields.forEach(field => setFieldValue(field, ec[field], false));
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
                                <label>Content</label>
                                <Field name="content" type="textarea" className={'form-control' + (errors.content && touched.content ? ' is-invalid' : '')} />
                                <ErrorMessage name="content" component="div" className="invalid-feedback" />
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

export { AddEditEC };