import { type PrimitiveType, useIntl } from "react-intl";

export type Values = Record<string, PrimitiveType> | undefined;

const useTranslation = (): ((id: string, values?: Values) => string) => {
  const intl = useIntl();
  return (id: string, values?: Values) => {
    return intl.formatMessage({ id, defaultMessage: id }, values);
  };
};

export default useTranslation;
