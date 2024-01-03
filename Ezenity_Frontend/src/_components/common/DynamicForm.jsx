import React, { useEffect, useState } from 'react';
import { Link, useParams, useHistory } from 'react-router-dom';
import { Formik, Field, Form, ErrorMessage } from 'formik';
import * as Yup from 'yup';

import { getService, alertService } from '@/_services';
import { secService, alertService } from '@/_services';

function DynamicForm({ entityType }) {
  const { id } = useParams();
  const history = useHistory();
  const isEditMode = !!id;
  const initialValues = {};
  const validationSchema = Yup.object().shape({});

  // Use the section service for CRUD operations
  const sectionService = secService;

  // Function to fetch the initial values and validation schema based on entityType
  useEffect(() => {
    const entityDetails = getEntityDetails(entityType);
    initialValues = entityDetails.initialValues;
    validationSchema = entityDetails.validationSchema;
  }, [entityType]);

  // Dynamic service based on entityType
  const service = getService(entityType);

  async function onSubmit(fields, { setStatus, setSubmitting }) {
    setStatus();
    if (isEditMode) {
      updateEntity(id, fields, setSubmitting);
    } else {
      createEntity(fields, setSubmitting);
    }
  }

  async function createEntity(fields, setSubmitting) {
    try {
      await sectionService.create(fields); // Use sectionService for creating a new section
      alertService.success(`${entityType} added successfully`, { keepAfterRouteChange: true });
      history.push('/overview'); // Redirect to the overview page after successful creation
    } catch (error) {
      setSubmitting(false);
      alertService.error(error.message);
    }
  }

  async function updateEntity(id, fields, setSubmitting) {
    try {
      await service.update(id, fields);
      alertService.success(`${entityType} updated successfully`, { keepAfterRouteChange: true });
      history.goBack(); // Redirect back to the previous page after successful update
    } catch (error) {
      setSubmitting(false);
      alertService.error(error.message);
    }
  }

  return (
    <Formik initialValues={initialValues} validationSchema={validationSchema} onSubmit={onSubmit}>
      {({ errors, touched, isSubmitting }) => {
        const [entity, setEntity] = useState(null);

        useEffect(() => {
          if (isEditMode) {
            // Fetch the entity and set form fields
            service.getById(id).then((data) => setEntity(data));
          }
        }, [isEditMode, id, service]);

        return (
          <Form>
            <h1>{isEditMode ? `Edit ${entityType}` : `Add ${entityType}`}</h1>
            {/* Render dynamic form fields here based on your entity's fields */}
            {Object.entries(initialValues).map(([fieldName, initialValue]) => (
              <div className="form-group" key={fieldName}>
                <label>{fieldName}</label>
                <Field
                  name={fieldName}
                  type={typeof initialValue === 'number' ? 'number' : 'text'}
                  className={`form-control${errors[fieldName] && touched[fieldName] ? ' is-invalid' : ''}`}
                />
                <ErrorMessage name={fieldName} component="div" className="invalid-feedback" />
              </div>
            ))}
            <div className="form-group mt-1">
              <button type="submit" disabled={isSubmitting} className="btn btn-sm btn-primary me-1">
                {isSubmitting && <span className="spinner-border spinner-border-sm mr-1"></span>}
                Save
              </button>
              <Link to="/overview" className="btn btn-sm btn-danger">
                Cancel
              </Link>
            </div>
          </Form>
        );
      }}
    </Formik>
  );
}

// Mock function to return initial values and validation schema for different entities
function getEntityDetails(entityType) {
  switch (entityType) {
    case 'EC':
      return {
        initialValues: {
          title: '',
          content: '',
        },
        validationSchema: Yup.object().shape({
          title: Yup.string().required('Title is required'),
          content: Yup.string().required('Content is required'),
        }),
      };
    case 'ProjectHistory':
      return {
        initialValues: {
          title: '',
          date: '',
          location: '',
          descriptions: [],
        },
        validationSchema: Yup.object().shape({
          title: Yup.string().required('Title is required'),
          date: Yup.string().required('Date is required'),
          location: Yup.string().required('Location is required'),
          descriptions: Yup.array().of(
            Yup.object().shape({
              descriptionList: Yup.string().min(4, 'Must contain at least 4 characters').required('Required'),
            })
          ),
        }),
      };
    // Add cases for other entity types as needed
    default:
      return {
        initialValues: {},
        validationSchema: Yup.object().shape({}),
      };
  }
}

export default DynamicForm;
