import React from "react";

export function ImgWithFallback({
  src,
  fallback,
  type = 'image/webp',
  ...delegated
}) {
  return (
    <picture>
      <source srcSet={src} type={type} />
      <img src={fallback} {...delegated} />
    </picture>
  );
};