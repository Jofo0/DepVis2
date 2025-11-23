import { useParams } from "react-router-dom";

export const useGetProjectId = () => {
  const { id } = useParams<{ id: string }>();
  return id!;
};
