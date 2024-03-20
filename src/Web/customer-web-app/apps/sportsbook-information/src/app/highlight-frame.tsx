import styles from "./highlight-frame.module.scss";

export function HighlightFrame ({header, content}: {header: string, content: string}) {
  return <div className={styles.container}>
    <div className={styles.header}>{header}</div>
    <div className={styles.content}>{content}</div>
  </div>
}
