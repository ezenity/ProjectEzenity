import React, { useState, useEffect } from 'react';
import { useHistory, useParams } from 'react-router-dom';
import { secService } from '@/_services/sec.service';

function AddEditSection() {
  const history = useHistory();
  const { id } = useParams();
  const isEditMode = !!id;
  const [title, setTitle] = useState('');
  const [content, setContent] = useState('');
  const [layout, setLayout] = useState('default'); // Add state for layout

  useEffect(() => {
    if (isEditMode) {
      // Fetch section data for editing
      secService.getById(id).then((data) => {
        setTitle(data.title);
        setContent(data.content);
        setLayout(data.layout); // Set layout state for editing
      });
    }
  }, [id, isEditMode]);

  const handleSubmit = async () => {
    const sectionData = { title, content, layout };

    try {
      if (isEditMode) {
        await secService.update(id, sectionData);
      } else {
        await secService.create(sectionData);
      }
      history.push('/about/career'); // Redirect back to the Career page after successful submission
    } catch (error) {
      // Handle error
    }
  };

  return (
    <div>
      <h1>{isEditMode ? 'Edit Section' : 'Add Section'}</h1>
      <div>
        <label>Title:</label>
        <input type="text" value={title} onChange={(e) => setTitle(e.target.value)} />
      </div>
      <div>
        <label>Content:</label>
        <textarea value={content} onChange={(e) => setContent(e.target.value)} />
      </div>
      <div>
        <label>Layout:</label>
        <select value={layout} onChange={(e) => setLayout(e.target.value)}>
          <option value="default">Default</option>
          <option value="sidebar">Sidebar</option>
        </select>
      </div>
      <button className="btn btn-sm btn-primary" onClick={handleSubmit}>
        {isEditMode ? 'Update Section' : 'Add Section'}
      </button>
      <button className="btn btn-sm btn-primary" onClick={() => history.push('/about/career')}>
        Cancel
      </button>
    </div>
  );
}

export { AddEditSection };